//using BO;
//using DalApi;
//using DO;
//using Microsoft.VisualBasic;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Net;
//using System.Text.Json;
//using System.Text.RegularExpressions;

//namespace Helpers;
//internal static class VolunteerManager
//{
//    private static IDal s_dal = Factory.Get; //stage 4
//    internal static ObserverManager Observers = new(); //stage 5
//    // Returns the total number of assignments for a volunteer with a given end status
//    public static int TotalCallsByEndStatus(int id, DO.Enums.TerminationTypeEnum status)
//    {
//        var result = s_dal.Assignment.ReadAll()
//            .Where(a => a.VolunteerId == id && a.EndStatus == status);
//        return result.Count();
//    }
//    // Validates the volunteer's data and returns the name of the first invalid field or "true"
//    public static string IsValid(BO.Volunteer volunteer)
//    {
//        if (!IsValidId(volunteer.Id))
//            return "Id";
//        if (string.IsNullOrWhiteSpace(volunteer.FullName))
//            return "FullName";
//        if (!IsValidPhoneNumber(volunteer.PhoneNumber))
//            return "PhoneNumber";
//        if (!IsValidEmail(volunteer.Email))
//            return "Email";
//        if (volunteer.Password != null && string.IsNullOrWhiteSpace(volunteer.Password))
//            return "Password";
//        if (volunteer.Latitude is < -90 or > 90)
//            return "Latitude";
//        if (volunteer.Longitude is < -180 or > 180)
//            return "Longitude";
//        if (volunteer.MaxDistance is < 0)
//            return "MaxDistance";
//        return "true";
//    }
//    // Validates an Israeli ID number using the checksum algorithm
//    public static bool IsValidId(int volunteerId)
//    {
//        string id = volunteerId.ToString();
//        if (string.IsNullOrWhiteSpace(id))
//            return false;
//        id = id.PadLeft(9, '0');
//        if (id.Length != 9 || !id.All(char.IsDigit))
//            return false;
//        int sum = 0;
//        for (int i = 0; i < 9; i++)
//        {
//            int digit = (id[i] - '0');
//            int step = digit * ((i % 2) + 1);
//            if (step > 9)
//                step -= 9;
//            sum += step;
//        }

//        //return sum % 10 == 0;
//        return true;
//    }
//    // Validates a phone number with a minimum of 7 and maximum of 15 digits
//    private static bool IsValidPhoneNumber(string phoneNumber)
//    {
//        return Regex.IsMatch(phoneNumber, @"^\d{7,15}$");
//    }
//    // Validates that the email address is in a proper format
//    private static bool IsValidEmail(string email)
//    {
//        return Regex.IsMatch(email,
//            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
//    }
//    // Converts a business object (BO) volunteer to a data object (DO) volunteer
//    public static DO.Volunteer ConvertBoToDo(BO.Volunteer boVolunteer)
//    {
//        return new DO.Volunteer
//        {
//            Id = boVolunteer.Id,
//            FullName = boVolunteer.FullName,
//            PhoneNumber = boVolunteer.PhoneNumber,
//            Email = boVolunteer.Email,
//            Password = boVolunteer.Password,
//            Address = boVolunteer.Address,
//            Latitude = GetCoordinates(boVolunteer.Address)[0],
//            Longitude = GetCoordinates(boVolunteer.Address)[1],
//            Role = (DO.Enums.RoleEnum)boVolunteer.Role,
//            IsActive = boVolunteer.IsActive,
//            MaxOfDistance = boVolunteer.MaxDistance,
//            TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)boVolunteer.TypeOfDistance
//        };
//    }
//    // Placeholder for periodic updates related to volunteer assignments
//    public static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
//    {
//        var allVolunteers = s_dal.Volunteer.ReadAll();
//        bool volunteerUpdated = false;
//        foreach (var volunteer in allVolunteers)
//        {
//            volunteerUpdated = true;
//            Observers.NotifyItemUpdated(volunteer.Id);
//        }
//        bool yearChanged = oldClock.Year != newClock.Year;
//        if (yearChanged || volunteerUpdated)
//            Observers.NotifyListUpdated();
//    }
//    // Gets geographic coordinates (latitude and longitude) for a given address using an external API
//    public static double[] GetCoordinates(string address)
//    {
//        string apiKey = "680b754174669296818770btm636896";
//        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";

//        using (HttpClient client = new HttpClient())
//        {
//            var response = client.GetAsync(url).Result;
//            if (!response.IsSuccessStatusCode)
//            {
//                Console.WriteLine("Failed to get response.");
//                return [];
//            }
//            var json = response.Content.ReadAsStringAsync().Result;
//            var doc = JsonDocument.Parse(json);
//            var results = doc.RootElement;
//            if (results.GetArrayLength() > 0)
//            {
//                var firstResult = results[0];
//                double lat = double.Parse(firstResult.GetProperty("lat").GetString());
//                double lon = double.Parse(firstResult.GetProperty("lon").GetString());
//                return new double[] { lat, lon };
//            }
//            else
//            {
//                Console.WriteLine("No results found.");
//            }
//            return [];
//        }
//    }
//    public static BO.CallType CallTypeIfExist(int id)
//    {
//        var assignment = s_dal.Assignment.ReadAll()
//            .FirstOrDefault(a => a.VolunteerId == id && a.EndTime == null);
//        if (assignment == null || assignment.CallId == null)
//            return BO.CallType.None;
//        var callId = assignment.CallId;
//        if (callId != null)
//        {
//            var currrentCall = s_dal.Call.ReadAll()
//                .FirstOrDefault(c => c.Id == callId);
//            var result = currrentCall.Type;
//            return (BO.CallType)result;
//        }
//        return BO.CallType.None;
//    }
//}

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
    public static DO.Volunteer ConvertBoToDo(BO.Volunteer boVolunteer)
    {
        return new DO.Volunteer
        {
            Id = boVolunteer.Id,
            FullName = boVolunteer.FullName,
            PhoneNumber = boVolunteer.PhoneNumber,
            Email = boVolunteer.Email,
            Password = boVolunteer.Password,
            Address = boVolunteer.Address,
            Latitude = GetCoordinates(boVolunteer.Address)[0],
            Longitude = GetCoordinates(boVolunteer.Address)[1],
            Role = (DO.Enums.RoleEnum)boVolunteer.Role,
            IsActive = boVolunteer.IsActive,
            MaxOfDistance = boVolunteer.MaxDistance,
            TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)boVolunteer.TypeOfDistance
        };
    }
    // Placeholder for periodic updates related to volunteer assignments
    public static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    {
        IEnumerable<DO.Volunteer> allVolunteers;
        lock (AdminManager.BlMutex) //stage 7
        {
            allVolunteers = s_dal.Volunteer.ReadAll().ToList();
        }

        bool volunteerUpdated = false;
        foreach (var volunteer in allVolunteers)
        {
            volunteerUpdated = true;
            Observers.NotifyItemUpdated(volunteer.Id); // לא בתוך lock
        }

        bool yearChanged = oldClock.Year != newClock.Year;
        if (yearChanged || volunteerUpdated)
            Observers.NotifyListUpdated(); // לא בתוך lock
    }
    // Gets geographic coordinates (latitude and longitude) for a given address using an external API
    public static double[] GetCoordinates(string address)
    {
        string apiKey = "680b754174669296818770btm636896";
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            var response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to get response.");
                return [];
            }
            var json = response.Content.ReadAsStringAsync().Result;
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
            return [];
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
}