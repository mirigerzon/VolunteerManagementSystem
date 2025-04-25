using BlApi;
using BO;
using DalApi;
using DO;

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
    public static List<CallInList> GetCallsList()
    {
        var calls = s_dal.Call.ReadAll();
        List<CallInList> callInList = calls.Select(call => new CallInList
        {
            Id = findAssignment(call.Id),
            CallId = call.Id,
            CallType = (CallType)call.Type,
            StartTime = call.StartTime,
            TimeLeft = call.MaxEndTime - s_dal.Config.Clock,
            LastVolunteerName = getLastVolunteerName(call.Id),
            TreatmentDuration = getTreatmentDuration(call.Id),
            Status = (CallStatus)call.Status,
        }).ToList();

        return callInList;
    }
    //public static BO.Call ConvertBoToDo(DO.Call doCall)
    //{
    //    return new BO.Call
    //    {
    //        Id = doCall.Id,
    //        Type = (CallType)doCall.Type
    //    };
    //}
    private static int findAssignment(int callId)
    {
        var a = s_dal.Assignment.Read(callId);
        return a.VolunteerId;
    }
    private static string getLastVolunteerName(int Id)
    {

    }
    private static bool getTreatmentDuration(int Id)
    {
        DO.Call doCall = s_dal.Call.Read(Id);
        BO.Call boCall = ConvertBoToDo(doCall);
        if (doCall.Status = "closed")
        {
            TimeSpan treatmentDuration = doCall.MaxEndTime - doCall.Ti;
        }
    }
    //public static void ValidateCallAndUpdateVolunteer(BO.Call call)
    //{
    //    if (call.StartTime.HasValue && call.MaxEndTime.HasValue)
    //    {
    //        if (call.MaxEndTime <= call.StartTime)
    //            throw new ArgumentException("MaxEndTime must be greater than StartTime.");
    //    }
    //    else
    //    {
    //        throw new ArgumentException("StartTime and MaxEndTime must have values.");
    //    }
    //    if (string.IsNullOrWhiteSpace(call.CallerAddress))
    //        throw new ArgumentException("Caller address must not be empty.");
    //    if (!IsValidCoordinate(call.Latitude, call.Longitude))
    //        throw new ArgumentException("Invalid coordinates: latitude and longitude must be within real-world range.");
    //    var volunteer = s_dal.Volunteer
    //        .ReadAll()
    //        .FirstOrDefault(v => v.Address == call.CallerAddress);
    //    if (volunteer != null)
    //    {
    //        var updatedVolunteer = new DO.Volunteer
    //        {
    //            Id = volunteer.Id,
    //            FullName = volunteer.FullName,
    //            PhoneNumber = volunteer.PhoneNumber,
    //            Email = volunteer.Email,
    //            Password = volunteer.Password,
    //            Address = volunteer.Address,
    //            Latitude = call.Latitude,
    //            Longitude = call.Longitude,
    //            Role = volunteer.Role,
    //            IsActive = volunteer.IsActive,
    //            MaxOfDistance = volunteer.MaxOfDistance,
    //            TypeOfDistance = volunteer.TypeOfDistance
    //        };
    //        s_dal.Volunteer.Update(updatedVolunteer);
    //    }
    //}
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
    public static DO.Volunteer ConvertBoToDo(BO.Volunteer boVolunteer)
    {

        var volunteer1 = s_dal.Volunteer
            .ReadAll()
            .FirstOrDefault(v => v.Address == call.CallerAddress);
            if (volunteer1 != null)
            {
                return new DO.Volunteer
                {
                    Id = boVolunteer.Id,
                    FullName = boVolunteer.FullName,
                    PhoneNumber = boVolunteer.PhoneNumber,
                    Email = boVolunteer.Email,
                    Password = boVolunteer.Password,
                    Address = boVolunteer.Address,
                    Latitude = call.Latitude,
                    Longitude = Call.Longitude,
                    Role = (DO.Enums.RoleEnum)boVolunteer.Role,
                    IsActive = boVolunteer.IsActive,
                    MaxOfDistance = boVolunteer.MaxDistance,
                    TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)boVolunteer.TypeOfDistance
                };
            }
        }