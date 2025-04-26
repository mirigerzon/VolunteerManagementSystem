using BlApi;
using BO;
using DalApi;
using DO;
using System.Text.Json;
using static DO.Enums;

namespace Helpers;
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
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
    private static int findAssignment(int callId)
    {
        var a = s_dal.Assignment.Read(callId);
        return a.VolunteerId;
    }

    #region GetCallInList
    //public static List<CallInList> GetCallsList()
    //{
    //    var calls = s_dal.Call.ReadAll();
    //    List<CallInList> callInList = calls.Select(call => new CallInList
    //    {
    //        Id = findAssignment(call.Id),
    //        CallId = call.Id,
    //        CallType = (CallType)call.Type,
    //        StartTime = call.StartTime,
    //        TimeLeft = call.MaxEndTime - s_dal.Config.Clock,
    //        LastVolunteerName = getLastVolunteerName(call.Id),
    //        TreatmentDuration = getTreatmentDuration(call.Id),
    //        Status = (CallStatus)call.Status,
    //    }).ToList();

    //    return callInList;
    //}
    //private static string getLastVolunteerName(int Id)
    //{

    //}

    //private static bool getTreatmentDuration(int Id)
    //{
    //    DO.Call doCall = s_dal.Call.Read(Id);
    //    BO.Call boCall = ConvertBoToDo(doCall);
    //    if (doCall.Status = "closed")
    //    {
    //        TimeSpan treatmentDuration = doCall.MaxEndTime - doCall.Ti;
    //    }
    //}
    #endregion

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
    }
    public static string IsValid(BO.Call call)
    {
        if (!IsValidId(call.Id))
            return "Id";

        if (string.IsNullOrWhiteSpace(call.Description))
            return "Description";

        if (string.IsNullOrWhiteSpace(call.CallerAddress))
            return "CallerAddress";

        if (call.StartTime == null)
            return "StartTime";

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
    public static bool IsValidId(int id)
    {
        string idString = id.ToString().PadLeft(9, '0');
        if (idString.Length != 9)
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = idString[i] - '0';
            int multiplied = digit * ((i % 2) + 1);
            if (multiplied > 9)
                multiplied -= 9;
            sum += multiplied;
        }

        return sum % 10 == 0;
    }
    private static bool IsValidCoordinate(double? latitude, double? longitude)
    {
        return latitude >= -90 && latitude <= 90 &&
               longitude >= -180 && longitude <= 180;
    }
    private static readonly HttpClient client = new HttpClient();
    private static readonly string apiKey = "680b754174669296818770btm636896";
    public static (double Latitude, double Longitude) GetLatitudLongitute(string address)
    {
        try
        {
            string url = $"https://us1.locationiq.com/v1/search?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            string responseBody = response.Content.ReadAsStringAsync().Result;

            JsonDocument jsonDoc = JsonDocument.Parse(responseBody);

            if (jsonDoc.RootElement.GetArrayLength() > 0)
            {
                var firstResult = jsonDoc.RootElement[0];
                double lat = firstResult.GetProperty("lat").GetDouble();
                double lon = firstResult.GetProperty("lon").GetDouble();
                return (Latitude: lat, Longitude: lon);
            }

            throw new Exception("No results found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
    public static ClosedCallInList ConvertToClosedCallInList(DO.Call call)
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
    public static double CalculateDistance(double? VLongitude, double? VLatitude, double? CLongitude, double? CLatitude)
    {
        // רדיוס כדור הארץ בקילומטרים
        const double EarthRadius = 6371;
        // בדיקת תקינות קואורדינטות המתנדב
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
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}