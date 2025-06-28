using BO;
using DalApi;
using DO;
using System.Net;
using System.Text.Json;
using static DO.Enums;
namespace Helpers;
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5
    // Returns a specific field value from a Call object based on the provided filter.
    public static object GetFieldValue(BO.Call call, CallFieldFilter field)
    {
        return field switch
        {
            CallFieldFilter.Status => call.Status,
            CallFieldFilter.CallerAddress => call.CallerAddress,
            CallFieldFilter.StartTime => call.StartTime,
            _ => call.Id
        };
    }
    // Validates a call and asynchronously updates the coordinates of the volunteer at the same address.
    public static void ValidateCallAndUpdateVolunteer(BO.Call call)
    {
        if (call.StartTime.HasValue && call.MaxEndTime.HasValue)
        {
            if (call.MaxEndTime <= call.StartTime)
                throw new ArgumentException("MaxEndTime must be greater than StartTime.");
        }
        else
        {
            throw new ArgumentException("StartTime and MaxEndTime must have values.");
        }
        if (string.IsNullOrWhiteSpace(call.CallerAddress))
            throw new ArgumentException("Caller address must not be empty.");
        if (!IsValidCoordinate(call.Latitude, call.Longitude))
            throw new ArgumentException("Invalid coordinates: latitude and longitude must be within real-world range.");
        lock (AdminManager.BlMutex)
        {
            var volunteer = s_dal.Volunteer
                .ReadAll()
                .FirstOrDefault(v => v.Address == call.CallerAddress);
            if (volunteer != null)
            {
                var updatedVolunteer = new DO.Volunteer
                {
                    Id = volunteer.Id,
                    FullName = volunteer.FullName,
                    PhoneNumber = volunteer.PhoneNumber,
                    Email = volunteer.Email,
                    Password = volunteer.Password,
                    Address = volunteer.Address,
                    Latitude = call.Latitude,
                    Longitude = call.Longitude,
                    Role = volunteer.Role,
                    IsActive = volunteer.IsActive,
                    MaxOfDistance = volunteer.MaxOfDistance,
                    TypeOfDistance = volunteer.TypeOfDistance
                };

                s_dal.Volunteer.Update(updatedVolunteer);
            }
            if (volunteer != null)
                Observers.NotifyItemUpdated(volunteer.Id);
        }
        _ = UpdateVolunteerCoordinatesByCallAddressAsync(call);
    }
    // Performs validation on a Call object and returns the name of the first invalid field, or "true" if valid.
    public static string IsValid(BO.Call call)
    {
        if (string.IsNullOrWhiteSpace(call.Description))
            return "Description";

        if (string.IsNullOrWhiteSpace(call.CallerAddress))
            return "CallerAddress";

        if (call.MaxEndTime == null)
            return "MaxEndTime";

        if (call.MaxEndTime < call.StartTime)
            return "MaxEndTime";

        if (call.Latitude is < -90 or > 90)
            return "Latitude";

        if (call.Longitude is < -180 or > 180)
            return "Longitude";

        return "true";
    }
    // Checks if the given latitude and longitude are within valid geographic bounds.
    private static bool IsValidCoordinate(double? latitude, double? longitude)
    {
        return latitude >= -90 && latitude <= 90 &&
               longitude >= -180 && longitude <= 180;
    }
    // Retrieves geographic coordinates (latitude and longitude) for a given address using a geocoding API.
    public static async Task<double[]> GetCoordinatesAsync(string address)
    {
        string apiKey = "680b754174669296818770btm636896";
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to get response.");
                return Array.Empty<double>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var results = doc.RootElement;
            if (results.GetArrayLength() > 0)
            {
                var firstResult = results[0];
                double lat = double.Parse(firstResult.GetProperty("lat").GetString());
                double lon = double.Parse(firstResult.GetProperty("lon").GetString());
                return new double[] { lat, lon };
            }
            else
            {
                Console.WriteLine("No results found.");
            }

            return Array.Empty<double>();
        }
    }
    public static async Task UpdateVolunteerCoordinatesByCallAddressAsync(BO.Call call)
    {
        if (string.IsNullOrWhiteSpace(call.CallerAddress))
            return;

        var coordinates = await GetCoordinatesAsync(call.CallerAddress);
        if (coordinates.Length == 2)
        {
            lock (AdminManager.BlMutex)
            {
                var volunteer = s_dal.Volunteer
                    .ReadAll()
                    .FirstOrDefault(v => v.Address == call.CallerAddress);

                if (volunteer != null)
                {
                    var updatedVolunteer = volunteer with
                    {
                        Latitude = coordinates[0],
                        Longitude = coordinates[1]
                    };
                    s_dal.Volunteer.Update(updatedVolunteer);
                }
            }
            // התראות מחוץ לנעילה
            var volId = s_dal.Volunteer.ReadAll()
                .FirstOrDefault(v => v.Address == call.CallerAddress)?.Id;
            if (volId != null)
                Observers.NotifyItemUpdated(volId.Value);
        }
    }
    public static async Task UpdateCoordinatesForCallAddressAsync(DO.Call doCall)
    {
        if (!string.IsNullOrWhiteSpace(doCall.CallerAddress))
        {
            var coordinates = await GetCoordinatesAsync(doCall.CallerAddress);
            if (coordinates.Length == 2)
            {
                doCall = doCall with
                {
                    Latitude = coordinates[0],
                    Longitude = coordinates[1]
                };

                lock (AdminManager.BlMutex)
                {
                    Factory.Get.Call.Update(doCall);
                }

                Observers.NotifyItemUpdated(doCall.Id);
            }
        }
    }
    // Converts a DO.Call object into a ClosedCallInList business object for closed call listings.
    public static ClosedCallInList ConvertToClosedCallInList(DO.Call call)
    {
        // ✳️ נעילה
        lock (AdminManager.BlMutex)
        {
            var assignment = s_dal.Assignment.Read(call.Id);
            return new ClosedCallInList
            {
                Id = call.Id,
                CallType = (CallType)call.Type,
                Address = call.CallerAddress,
                OpenedAt = call.StartTime,
                AssignedAt = assignment.ArrivalTime,
                ClosedAt = assignment.EndTime,
                ClosureType = (ClosureType?)assignment.EndStatus
            };
        }
    }
    // Calculates the distance in kilometers between a volunteer and a call based on their coordinates.
    public static double CalculateDistance(double? VLongitude, double? VLatitude, double? CLongitude, double? CLatitude)
    {
        const double EarthRadius = 6371;
        if (VLongitude == null || VLatitude == null)
        {
            throw new Exception("Volunteer latitude or longitude cannot be null.");
        }
        if (CLongitude == null || CLatitude == null)
        {
            throw new Exception("Call latitude or longitude cannot be null.");
        }

        double lat1 = DegreesToRadians(VLatitude.Value);
        double lon1 = DegreesToRadians(VLongitude.Value);
        double lat2 = DegreesToRadians(CLatitude.Value);
        double lon2 = DegreesToRadians(CLongitude.Value);
        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadius * c;
        return distance;
    }
    // Converts degrees to radians.
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
    // Converts a DO.Call object to a BO.Call object.
    public static BO.Call ConvertDoToBo(DO.Call call)
    {
        return new BO.Call
        {
            Id = call.Id,
            Type = (CallType)call.Type,
            Description = call.Description,
            CallerAddress = call.CallerAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            StartTime = call.StartTime,
            MaxEndTime = call.MaxEndTime,
            Status = (CallStatus)call.Status,
        };
    }
    // Converts a BO.Call object to a DO.Call object, including coordinate lookup based on address.
    public static async Task<DO.Call> ConvertBoToDoAsync(BO.Call call)
    {
        var coordinates = await GetCoordinatesAsync(call.CallerAddress);

        return new DO.Call
        {
            Id = call.Id,
            Type = (Enums.CallTypeEnum)call.Type,
            Description = call.Description,
            CallerAddress = call.CallerAddress,
            Latitude = coordinates.Length >= 2 ? coordinates[0] : null,
            Longitude = coordinates.Length >= 2 ? coordinates[1] : null,
            StartTime = call.StartTime,
            MaxEndTime = call.MaxEndTime,
            Status = (Enums.CallStatusEnum)call.Status,
        };
    }
    // Updates the status of all open calls whose MaxEndTime has passed to "Expired".
    public static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        List<int> updatedCallIds = new();
        lock (AdminManager.BlMutex)
        {
            var allCalls = s_dal.Call.ReadAll().ToList(); 
            foreach (var call in allCalls)
            {
                if (call.Status == CallStatusEnum.Open && call.MaxEndTime != null)
                {
                    if (newClock > call.MaxEndTime)
                    {
                        s_dal.Call.Update(new DO.Call
                        {
                            Id = call.Id,
                            Type = call.Type,
                            Description = call.Description,
                            CallerAddress = call.CallerAddress,
                            StartTime = call.StartTime,
                            MaxEndTime = call.MaxEndTime,
                            Status = CallStatusEnum.Expired
                        });
                        updatedCallIds.Add(call.Id); 
                    }
                }
            }
        }
        foreach (var id in updatedCallIds)
            Observers.NotifyItemUpdated(id);

        if (oldClock.Year != newClock.Year)
            Observers.NotifyListUpdated();
    }
}