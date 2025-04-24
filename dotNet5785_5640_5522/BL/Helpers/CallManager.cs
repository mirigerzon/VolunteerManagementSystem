using BlApi;
using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class callManager
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
            CallType = ,
            StartTime = call.StartTime,
            TimeLeft = call.MaxEndTime - s_dal.Config.Clock,
            LastVolunteerName = getLastVolunteerName(call.Id),
            //TreatmentDuration = ,
            Status = (CallStatus)call.Status,
            TotalAssignments =

        }).ToList();

        return callInList;
    }

    private static int findAssignment(int callId)
    {
        var a = s_dal.Assignment.Read(callId);
        return a.VolunteerId;
    }
    private static string getLastVolunteerName(int Id)
    {

    }
}
