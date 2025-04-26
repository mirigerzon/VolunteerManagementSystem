using BO;
using BlApi;
using DO;
using DalApi;
using Helpers;
using static DO.Enums;

namespace BlImplementation;
internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public int[] GetCallStatusCounts()
    {
        var calls = _dal.Call.ReadAll();
        return calls
            .GroupBy(c => (int)c.Status)
            .ToDictionary(g => g.Key, g => g.Count())
            .Select(kvp => kvp.Value)
            .ToArray();
    }
    public void CloseExpiredCalls()
    {
        var assignmentsData = _dal.Assignment.ReadAll();
        var allCalls = _dal.Call.ReadAll();
        var expiredCalls = allCalls
            .Where(c => c.MaxEndTime < _dal.Config.Clock && c.Status != CallStatusEnum.Closed)
            .ToList();
        foreach (var call in expiredCalls)
        {
            var assignment = assignmentsData.FirstOrDefault(a => a.CallId == call.Id);
            if (assignment == null)
            {
                var newAssignment = new DO.Assignment
                {
                    VolunteerId = 0,
                    CallId = call.Id,
                    ArrivalTime = null,
                    EndTime = _dal.Config.Clock,
                    EndStatus = Enums.TerminationTypeEnum.Expired
                };
                _dal.Assignment.Create(newAssignment);
            }
            else if (assignment.EndTime == null)
            {
                var updatedAssignment = new DO.Assignment
                (
                    assignment.Id,
                    assignment.VolunteerId,
                    assignment.CallId,
                    assignment.ArrivalTime,
                    _dal.Config.Clock
                );
                _dal.Assignment.Update(updatedAssignment);
            }
            _dal.Call.Update(new DO.Call
            {
                Id = call.Id,
                Status = CallStatusEnum.Expired,
                Type = call.Type,
                Description = call.Description,
                CallerAddress = call.CallerAddress,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                StartTime = call.StartTime,
                MaxEndTime = call.MaxEndTime
            });
        }
    }
    //public IEnumerable<CallInList> GetCallsList(CallFieldFilter? filterBy, object? filterValue, CallFieldFilter? sortBy)
    //{
    //    var calls = _dal.Call.ReadAll();
    //    if (filterBy != null && filterValue != null)
    //    {
    //        calls = calls.Where(c => Helpers.CallManager.GetFieldValue(c, filterBy.Value).Equals(filterValue));
    //    }
    //    if (sortBy != null)
    //    {
    //        calls = calls.OrderBy(c => Helpers.CallManager.GetFieldValue(c, sortBy.Value));
    //    }
    //    else
    //    {
    //        calls = calls.OrderBy(c => c.Id);
    //    }
    //    return calls;
    //}
    public BO.CallAssignInList GetCallDetails(int callId)
    {
        try
        {
            var callData = _dal.Call.Read(callId);
            var assignmentsData = _dal.Assignment.Read(callId);
            var volunteerData = _dal.Volunteer.Read(assignmentsData.VolunteerId);
            return new BO.CallAssignInList
            {
                VolunteerId = assignmentsData.VolunteerId,
                VolunteerName = volunteerData.FullName,
                StartTreatmentTime = assignmentsData.ArrivalTime,
                EndTreatmentTime = assignmentsData.EndTime,
                EndType = (AssignmentEndType)assignmentsData.EndStatus
            };
        }
        catch (Exception ex)
        {
            throw new Exception("CallAssignInList not found", ex);
        }
    }
    public void UpdateCall(BO.Call call)
    {
        try
        {
            var existingCall = _dal.Call.Read(call.Id);
            string checkValues = Helpers.CallManager.IsValid(call);
            if (checkValues == "true")
            {
                var newCall = new DO.Call
                {
                    Id = call.Id,
                    Type = (DO.Enums.CallTypeEnum)call.Type,
                    Description = call.Description,
                    CallerAddress = call.CallerAddress,
                    Latitude = CallManager.GetLatitudLongitute(call.CallerAddress).Latitude, // Get latitude for the address
                    Longitude = CallManager.GetLatitudLongitute(call.CallerAddress).Longitude, // Get longitude for the address
                    StartTime = call.StartTime,
                    MaxEndTime = call.MaxEndTime
                };
                _dal.Call.Update(newCall);
            }
            else
                throw new Exception(checkValues + " - this field is not valid");
        }
        catch (Exception ex)
        {
            throw new Exception("ERROR" + ex);
        }
    }
    public void DeleteCall(int callId)
    {
        DO.Call call;
        try
        {
            call = _dal.Call.Read(callId);
        }
        catch (Exception ex)
        {
            throw new KeyNotFoundException($"No call found with ID {callId}.", ex);
        }
        if (call.Status != CallStatusEnum.Open)
            throw new InvalidOperationException("Only calls with status 'Open' can be deleted.");
        bool hasAssignment = _dal.Assignment.ReadAll()
            .Any(a => a.CallId == callId);
        if (hasAssignment)
            throw new InvalidOperationException("Cannot delete a call that was assigned to a volunteer.");
        _dal.Call.Delete(callId);
    }
    public void CreateCall(BO.Call call)
    {
        string validationResult = Helpers.CallManager.IsValid(call);
        if (validationResult != "true")
            throw new ArgumentException($"Field '{validationResult}' is invalid.");
        try
        {
            DO.Call newCall = new DO.Call
            {
                Id = call.Id,
                Description = call.Description,
                CallerAddress = call.CallerAddress,
                StartTime = call.StartTime,
                MaxEndTime = call.MaxEndTime,
                Latitude = call.Latitude ?? 0,
                Longitude = call.Longitude ?? 0,
                Status = DO.Enums.CallStatusEnum.Open
            };
            _dal.Call.Create(newCall);
        }
        catch (Exception ex)
        {
            throw new Exception InvalidOperationException($"A call with ID {call.Id} already exists.", ex);
        }
    }
    public List<ClosedCallInList> GetClosedCallsOfVolunteer(
    int volunteerId,
    CallType? filterByType = null,
    ClosureType? sortByClosureType = null)
    {
        var allClosedCalls = _dal.Call.ReadAll()
            .Where(call =>
                call.Status == DO.Enums.CallStatusEnum.Closed &&
                _dal.Assignment.Read(call.Id).VolunteerId == volunteerId)
            .Select(call => Helpers.CallManager.ConvertToClosedCallInList(call));
        if (filterByType.HasValue)
        {
            allClosedCalls = allClosedCalls
                .Where(call => call.CallType == filterByType.Value);
        }
        if (sortByClosureType.HasValue)
        {
            allClosedCalls = allClosedCalls
                .OrderBy(call => call.ClosureType.HasValue ? call.ClosureType.Value : ClosureType.Treated);
        }
        else
        {
            allClosedCalls = allClosedCalls
                .OrderBy(call => call.Id);
        }
        return allClosedCalls.ToList();
    }
    public List<OpenCallInList> GetOpenCallsForVolunteer(
    int volunteerId,
    CallType? filterByType = null,
    OpenCallSortField? sortByField = null)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId);
        var openCalls = _dal.Call.ReadAll()
            .Where(call =>
                call.Status == DO.Enums.CallStatusEnum.Open ||
                call.Status == DO.Enums.CallStatusEnum.Aborted)
            .Select(call => new OpenCallInList
            {
                Id = call.Id,
                CallType = (CallType)call.Type,
                Description = call.Description,
                Address = call.CallerAddress,
                OpeningTime = call.StartTime,
                MaxFinishTime = call.MaxEndTime,
                DistanceFromVolunteer = Helpers.CallManager.CalculateDistance(
                    volunteer.Address, call.CallerAddress)
            });
        if (filterByType.HasValue)
        {
            openCalls = openCalls.Where(call => call.CallType == filterByType.Value);
        }
        if (sortByField.HasValue)
        {
            openCalls = sortByField.Value switch
            {
                OpenCallSortField.Id => openCalls.OrderBy(call => call.Id),
                OpenCallSortField.OpeningTime => openCalls.OrderBy(call => call.OpeningTime),
                OpenCallSortField.MaxFinishTime => openCalls.OrderBy(call => call.MaxFinishTime),
                OpenCallSortField.DistanceFromVolunteer => openCalls.OrderBy(call => call.DistanceFromVolunteer),
                _ => openCalls
            };
        }
        else
        {
            openCalls = openCalls.OrderBy(call => call.Id);
        }
        return openCalls.ToList();
    }
    public IEnumerable<CallInList> GetCallsList(CallFieldFilter? filterBy = null, object? filterValue = null, CallFieldFilter? sortBy = null)
    {
        throw new NotImplementedException();
    }
    public BO.Call Read(int id)
    {
        throw new NotImplementedException();
    }
    public IEnumerable<ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, CallStatus? statusFilter = null, CallSortField? sortBy = null)
    {
        throw new NotImplementedException();
    }
}