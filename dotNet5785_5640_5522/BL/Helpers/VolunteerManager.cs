using BlApi;
using BO;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
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
        if (IsValidId(volunteer.Id))
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
    public static bool IsValidId(int id)
    {
        string idString = id.ToString();
        idString = idString.PadLeft(9, '0');
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
            Latitude = boVolunteer.Latitude,
            Longitude = boVolunteer.Longitude,
            Role = (DO.Enums.RoleEnum)boVolunteer.Role,
            IsActive = boVolunteer.IsActive,
            MaxOfDistance = boVolunteer.MaxDistance,
            TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)boVolunteer.TypeOfDistance
        };
    }
}