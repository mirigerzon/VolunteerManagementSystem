using BO;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static int TotalCallsByEndStatus(int id, DO.Enums.TerminationTypeEnum status)
    {
        var result = s_dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == id && a.EndStatus == status);
        return result.Count();
    }
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

        return sum % 10 == 0;
    }
    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^\d{7,15}$"); // מינימום 7 ספרות, מקסימום 15
    }
    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    }
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
            Latitude = 1,
            Longitude = 1,
            Role = (DO.Enums.RoleEnum)boVolunteer.Role,
            IsActive = boVolunteer.IsActive,
            MaxOfDistance = boVolunteer.MaxDistance,
            TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)boVolunteer.TypeOfDistance
        };
    }
    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        throw new NotImplementedException();
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
}