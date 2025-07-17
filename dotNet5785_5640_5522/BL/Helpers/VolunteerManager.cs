using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5
    // Returns the total number of assignments for a volunteer with a given end status
    public static int TotalCallsByEndStatus(int id, DO.Enums.TerminationTypeEnum status)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var result = s_dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == id && a.EndStatus == status);
            return result.Count();
        }
    }
    // Validates the volunteer's data and returns the name of the first invalid field or "true"
    public static string IsValid(BO.Volunteer volunteer)
    {
        if (!IsValidId(volunteer.Id))
            return "Id";
        if (string.IsNullOrWhiteSpace(volunteer.FullName))
            return "FullName";
        if (!IsValidPhoneNumber(volunteer.PhoneNumber))
            return "PhoneNumber";
        if (!IsValidEmail(volunteer.Email))
            return "Email";
        if (volunteer.Password != null && string.IsNullOrWhiteSpace(volunteer.Password))
            return "Password";
        if (volunteer.Latitude is < -90 or > 90)
            return "Latitude";
        if (volunteer.Longitude is < -180 or > 180)
            return "Longitude";
        if (volunteer.MaxDistance is < 0)
            return "MaxDistance";
        return "true";
    }
    // Validates an Israeli ID number using the checksum algorithm
    public static bool IsValidId(int volunteerId)
    {
        string id = volunteerId.ToString();
        if (string.IsNullOrWhiteSpace(id))
            return false;
        id = id.PadLeft(9, '0');
        if (id.Length != 9 || !id.All(char.IsDigit))
            return false;
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = (id[i] - '0');
            int step = digit * ((i % 2) + 1);
            if (step > 9)
                step -= 9;
            sum += step;
        }

        //return sum % 10 == 0;
        return true;
    }
    // Validates a phone number with a minimum of 7 and maximum of 15 digits
    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^\d{7,15}$");
    }
    // Validates that the email address is in a proper format
    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    }
    // Converts a business object (BO) volunteer to a data object (DO) volunteer
    public static async Task<DO.Volunteer> ConvertBoToDoAsync(BO.Volunteer boVolunteer)
    {
        var coordinates = await GetCoordinatesAsync(boVolunteer.Address);
        return new DO.Volunteer
        {
            Id = boVolunteer.Id,
            FullName = boVolunteer.FullName,
            PhoneNumber = boVolunteer.PhoneNumber,
            Email = boVolunteer.Email,
            Password = boVolunteer.Password,
            Address = boVolunteer.Address,
            Latitude = coordinates.Length >= 2 ? coordinates[0] : null,
            Longitude = coordinates.Length >= 2 ? coordinates[1] : null,
            Role = (DO.Enums.RoleEnum)boVolunteer.Role,
            IsActive = boVolunteer.IsActive,
            MaxOfDistance = boVolunteer.MaxDistance,
            TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)boVolunteer.TypeOfDistance
        };
    }
    // Converts a business object (DO) volunteer to a data object (BO) volunteer
    public static BO.Volunteer ConvertDoToBo(DO.Volunteer doVolunteer)
    {
        return new BO.Volunteer
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            Password = doVolunteer.Password,
            Address = doVolunteer.Address,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Role = (BO.UserRole)doVolunteer.Role,
            IsActive = doVolunteer.IsActive,
            MaxDistance = doVolunteer.MaxOfDistance,
            TypeOfDistance = (BO.TypeOfDistance?)doVolunteer.TypeOfDistance,
            TotalHandledCalls = VolunteerManager.TotalCallsByEndStatus(doVolunteer.Id, DO.Enums.TerminationTypeEnum.Treated),
            TotalCanceledCalls = VolunteerManager.TotalCallsByEndStatus(doVolunteer.Id, DO.Enums.TerminationTypeEnum.SelfCancelled) + VolunteerManager.TotalCallsByEndStatus(doVolunteer.Id, DO.Enums.TerminationTypeEnum.ManagerCancelled),
            ExpiredCallsCount = VolunteerManager.TotalCallsByEndStatus(doVolunteer.Id, DO.Enums.TerminationTypeEnum.Expired),
            CurrentCall = null
        };
    }
    // Placeholder for periodic updates related to volunteer assignments
    public static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    {
        IEnumerable<DO.Volunteer> allDoVolunteers;
        lock (AdminManager.BlMutex)
        {
            allDoVolunteers = s_dal.Volunteer.ReadAll().ToList();
        }

        bool listUpdated = false;

        foreach (var doVolunteer in allDoVolunteers)
        {
            var volunteer = VolunteerManager.ConvertDoToBo(doVolunteer);
            bool updated = false;

            // איפוס שנתי של ExpiredCallsCount
            if (oldClock.Year != newClock.Year && volunteer.ExpiredCallsCount != 0)
            {
                volunteer.ExpiredCallsCount = 0;
                updated = true;
            }

            if (volunteer.Role == UserRole.Admin && volunteer.TotalCanceledCalls > 10)
            {
                volunteer.IsActive = false;
                updated = true;
            }

            if (updated)
            {
                var updatedDo = VolunteerManager.ConvertBoToDoAsync(volunteer).GetAwaiter().GetResult();
                lock (AdminManager.BlMutex)
                {
                    s_dal.Volunteer.Update(updatedDo);
                }
                Observers.NotifyItemUpdated(volunteer.Id);
                listUpdated = true;
            }
        }

        if (listUpdated || oldClock.Year != newClock.Year)
            Observers.NotifyListUpdated();
    }
    // Gets geographic coordinates (latitude and longitude) for a given address using an external API
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
    private static async Task UpdateVolunteerCoordinatesAsync(DO.Volunteer doVolunteer)
    {
        if (!string.IsNullOrWhiteSpace(doVolunteer.Address))
        {
            var coordinates = await GetCoordinatesAsync(doVolunteer.Address);
            if (coordinates.Length == 2)
            {
                var updatedVolunteer = doVolunteer with
                {
                    Latitude = coordinates[0],
                    Longitude = coordinates[1]
                };

                lock (AdminManager.BlMutex)
                {
                    s_dal.Volunteer.Update(updatedVolunteer);
                }
                Observers.NotifyItemUpdated(updatedVolunteer.Id);
                Observers.NotifyListUpdated();
            }
        }
    }
    public static void UpdateVolunteer(BO.Volunteer boVolunteer)
    {
        string valid = IsValid(boVolunteer);
        if (valid != "true")
            throw new BO.BlInvalidException($"Invalid field: {valid}");
        DO.Volunteer doVolunteer = new DO.Volunteer
        {
            Id = boVolunteer.Id,
            FullName = boVolunteer.FullName,
            PhoneNumber = boVolunteer.PhoneNumber,
            Email = boVolunteer.Email,
            Password = boVolunteer.Password,
            Address = boVolunteer.Address,
            Latitude = null,
            Longitude = null,
            Role = (DO.Enums.RoleEnum)boVolunteer.Role,
            IsActive = boVolunteer.IsActive,
            MaxOfDistance = boVolunteer.MaxDistance,
            TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)boVolunteer.TypeOfDistance
        };
        try
        {
            lock (AdminManager.BlMutex)
            {
                s_dal.Volunteer.Update(doVolunteer);
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist", ex);
        }
        Observers.NotifyItemUpdated(doVolunteer.Id);
        Observers.NotifyListUpdated();
        _ = UpdateVolunteerCoordinatesAsync(doVolunteer);
    }
    public static async Task UpdateCoordinatesForVolunteerAddressAsync(DO.Volunteer doVolunteer)
    {
        if (!string.IsNullOrWhiteSpace(doVolunteer.Address))
        {
            var coordinates = await GetCoordinatesAsync(doVolunteer.Address);
            if (coordinates.Length == 2)
            {
                doVolunteer = doVolunteer with
                {
                    Latitude = coordinates[0],
                    Longitude = coordinates[1]
                };
                lock (AdminManager.BlMutex)
                {
                    Factory.Get.Volunteer.Update(doVolunteer);
                }
                Observers.NotifyItemUpdated(doVolunteer.Id);
            }
        }
    }
    // Returns the current call type if the volunteer is currently assigned to a call
    public static BO.CallType CallTypeIfExist(int id)
    {
        DO.Assignment assignment;
        lock (AdminManager.BlMutex) //stage 7
        {
            assignment = s_dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.VolunteerId == id && a.EndTime == null);
        }

        if (assignment == null || assignment.CallId == null)
            return BO.CallType.None;

        var callId = assignment.CallId;
        if (callId != null)
        {
            DO.Call currrentCall;
            lock (AdminManager.BlMutex) //stage 7
            {
                currrentCall = s_dal.Call.ReadAll()
                    .FirstOrDefault(c => c.Id == callId);
            }

            var result = currrentCall.Type;
            return (BO.CallType)result;
        }

        return BO.CallType.None;
    }
    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;
    internal static void SimulateVolunteerActivity() // stage 7
    {
        Thread.CurrentThread.Name = $"VolunteerSimulator{++s_simulatorCounter}";

        List<DO.Volunteer> activeVolunteers;
        List<int> volunteersToNotify = new();

        lock (AdminManager.BlMutex)
        {
            activeVolunteers = s_dal.Volunteer.ReadAll(v => v.IsActive).ToList();
        }

        foreach (var volunteer in activeVolunteers)
        {
            if (TryAssignCallToVolunteer(volunteer))
            {
                volunteersToNotify.Add(volunteer.Id);
            }
            else if (TryCompleteOrCancelAssignment(volunteer))
            {
                volunteersToNotify.Add(volunteer.Id);
            }
        }

        foreach (var id in volunteersToNotify)
            Observers.NotifyItemUpdated(id);
    }
    private static bool TryAssignCallToVolunteer(DO.Volunteer volunteer)
    {
        DO.Assignment? currentAssignment;
        lock (AdminManager.BlMutex)
        {
            currentAssignment = s_dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.VolunteerId == volunteer.Id && a.EndTime == null);
        }
        if (currentAssignment != null) return false;
        if (s_rand.NextDouble() >= 0.2) return false;
        List<DO.Call> openCalls;
        lock (AdminManager.BlMutex)
        {
            openCalls = s_dal.Call.ReadAll(c =>
                c.Status == DO.Enums.CallStatusEnum.Open &&
                c.Latitude != null && c.Longitude != null
            ).ToList();
        }
        var suitableCalls = openCalls
            .Where(call =>
                CallManager.CalculateDistance(
                    volunteer.Longitude, volunteer.Latitude,
                    call.Longitude, call.Latitude
                ) <= volunteer.MaxOfDistance)
            .ToList();
        if (!suitableCalls.Any()) return false;
        var selectedCall = suitableCalls[s_rand.Next(suitableCalls.Count)];
        var newAssignment = new DO.Assignment
        {
            VolunteerId = volunteer.Id,
            CallId = selectedCall.Id,
            ArrivalTime = DateTime.Now,
            EndTime = null,
            EndStatus = null
        };
        lock (AdminManager.BlMutex)
        {
            s_dal.Assignment.Create(newAssignment);
            var updatedCall = selectedCall with { Status = DO.Enums.CallStatusEnum.InTreatment };
            s_dal.Call.Update(updatedCall);
        }
        return true;
    }
    private static bool TryCompleteOrCancelAssignment(DO.Volunteer volunteer)
    {
        DO.Assignment? currentAssignment;
        lock (AdminManager.BlMutex)
        {
            currentAssignment = s_dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.VolunteerId == volunteer.Id && a.EndTime == null);
        }

        if (currentAssignment == null) return false;

        DO.Call currentCall;
        lock (AdminManager.BlMutex)
        {
            currentCall = s_dal.Call.Read(currentAssignment.CallId);
        }

        TimeSpan elapsed = DateTime.Now - currentAssignment.ArrivalTime!.Value;

        double distance = CallManager.CalculateDistance(
            volunteer.Longitude, volunteer.Latitude,
            currentCall.Longitude, currentCall.Latitude
        );

        double minDuration = distance * 2 + s_rand.Next(2, 6); // דקות

        if (elapsed.TotalMinutes >= minDuration)
        {
            var updatedAssignment = currentAssignment with
            {
                EndTime = DateTime.Now,
                EndStatus = DO.Enums.TerminationTypeEnum.Treated
            };

            lock (AdminManager.BlMutex)
            {
                s_dal.Assignment.Update(updatedAssignment);

                var updatedCall = currentCall with
                {
                    Status = DO.Enums.CallStatusEnum.Closed
                };
                s_dal.Call.Update(updatedCall);
            }

            return true;
        }
        else if (s_rand.NextDouble() < 0.1)
        {
            lock (AdminManager.BlMutex)
            {
                s_dal.Assignment.Delete(currentAssignment.Id);

                var updatedCall = currentCall with
                {
                    Status = DO.Enums.CallStatusEnum.Open
                };
                s_dal.Call.Update(updatedCall);
            }

            return true;
        }

        return false;
    }
}
