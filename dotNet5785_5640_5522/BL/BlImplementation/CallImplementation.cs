using BO;
using BlApi;
using DO;
using DalApi;
using Helpers;
using static DO.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlImplementation;
internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public int[] GetCallStatusCounts()
    {
        try
        {
            var calls = _dal.Call.ReadAll();
            return calls
                .GroupBy(c => (int)c.Status)
                .ToDictionary(g => g.Key, g => g.Count())
                .Select(kvp => kvp.Value)
                .ToArray();
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BlXMLFileLoadCreateException("Failed to get call status counts", ex);
        }
    }
    public void CloseExpiredCalls()
    {
        try
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
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to close expired calls.", ex);
        }
    }
    public IEnumerable<CallInList> GetCallsList(CallFieldFilter? filterBy = null, object? filterValue = null, CallFieldFilter? sortBy = null)
    {
        //var data = _dal.Call.ReadAll().Select(Helpers.CallManager.ConvertDoToBo).ToList();
        List<BO.Call> calls = _dal.Call.ReadAll().Select(Helpers.CallManager.ConvertDoToBo).ToList();
        var callInLists = calls.Select(call =>
        {
            var lastAssign = call.Assignments?
                .OrderByDescending(a => a.EndTreatmentTime)
                .FirstOrDefault();

            return new CallInList
            {
                Id = call.Id,
                CallId = call.Id,
                CallType = call.Type,
                StartTime = call.StartTime,
                Status = call.Status,
                TimeLeft = call.MaxEndTime.HasValue ? call.MaxEndTime.Value - DateTime.Now : null,
                LastVolunteerName = lastAssign?.VolunteerName,
                TreatmentDuration = (lastAssign?.EndTreatmentTime.HasValue == true && lastAssign?.StartTreatmentTime.HasValue == true)
                                     ? lastAssign.EndTreatmentTime - lastAssign.StartTreatmentTime
                                     : null,
                TotalAssignments = call.Assignments?.Count ?? 0
            };
        }).ToList();
        if (filterBy.HasValue && filterValue != null)
        {
            callInLists = callInLists.Where(call =>
            {
                return filterBy.Value switch
                {
                    CallFieldFilter.Id => call.Id == Convert.ToInt32(filterValue),
                    CallFieldFilter.Type => call.CallType.Equals((CallType)filterValue),
                    CallFieldFilter.Status => call.Status.Equals((CallStatus)filterValue),
                    CallFieldFilter.StartTime => call.StartTime?.Date == Convert.ToDateTime(filterValue).Date,
                    CallFieldFilter.MaxEndTime => call.TimeLeft.HasValue &&
                                                  (DateTime.Now + call.TimeLeft.Value).Date == Convert.ToDateTime(filterValue).Date,
                    _ => true
                };
            }).ToList();
        }
        callInLists = (sortBy ?? CallFieldFilter.Id) switch
        {
            CallFieldFilter.Id => callInLists.OrderBy(call => call.Id).ToList(),
            CallFieldFilter.Type => callInLists.OrderBy(call => call.CallType).ToList(),
            CallFieldFilter.Status => callInLists.OrderBy(call => call.Status).ToList(),
            CallFieldFilter.StartTime => callInLists.OrderBy(call => call.StartTime).ToList(),
            CallFieldFilter.MaxEndTime => callInLists.OrderBy(call => call.TimeLeft).ToList(),
            _ => callInLists.OrderBy(call => call.Id).ToList()
        };

        return callInLists;
    }
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            var callData = _dal.Call.Read(callId);
            var assignmentsData = _dal.Assignment.Read(callId) ?? null;
            var volunteerData = assignmentsData != null ? _dal.Volunteer.Read(assignmentsData.VolunteerId) : null;
            List<CallAssignInList> Assignments = null;
            if (assignmentsData != null && volunteerData != null)
            {
                Assignments = new List<CallAssignInList>
                {
                   new BO.CallAssignInList
                   {
                       VolunteerId = assignmentsData.VolunteerId,
                       VolunteerName = volunteerData.FullName,
                       StartTreatmentTime = assignmentsData.ArrivalTime,
                       EndTreatmentTime = assignmentsData.EndTime,
                       EndType = (AssignmentEndType)assignmentsData.EndStatus
                   }
                };
            }
            return new BO.Call
            {
                Id = callId,
                Type = (CallType)callData.Type,
                Description = callData.Description,
                CallerAddress = callData.CallerAddress,
                Latitude = callData.Latitude,
                Longitude = callData.Longitude,
                StartTime = callData.StartTime,
                MaxEndTime = callData.MaxEndTime,
                Status = (CallStatus)callData.Status,
                Assignments = Assignments
            };
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException("CallAssignInList not found.", ex);
        }
    }
    public void Update(BO.Call call)
    {
        try
        {
            var existingCall = _dal.Call.Read(call.Id);
            string checkValues = Helpers.CallManager.IsValid(call);
            if (checkValues == "true")
            {
                _dal.Call.Update(Helpers.CallManager.ConvertBoToDo(call));
            }
            else
                throw new BlInvalidException(checkValues + " - this field is not valid");
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to update call.", ex);
        }
    }
    public void Delete(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId);
            if (call.Status != CallStatusEnum.Open)
                throw new BlInvalidException("Only calls with status 'Open' can be deleted.");
            bool hasAssignment = _dal.Assignment.ReadAll()
                .Any(a => a.CallId == callId);
            if (hasAssignment)
                throw new BlInvalidException("Cannot delete a call that was assigned to a volunteer.");
            _dal.Call.Delete(callId);
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException($"No call found with ID {callId}.", ex);
        }
    }
    public void Create(BO.Call call)
    {
        string validationResult = Helpers.CallManager.IsValid(call);
        if (validationResult != "true")
            throw new BlInvalidException($"Field '{validationResult}' is invalid.");
        try
        {
            double? latitude = 1; //Helpers.CallManager.GetLatitudLongitute(call.CallerAddress).Latitude;
            double? longitude = 1; //Helpers.CallManager.GetLatitudLongitute(call.CallerAddress).Longitude;

            DO.Call newCall = new DO.Call
            {
                Id = _dal.Config.GetNextCallId(),
                Description = call.Description,
                CallerAddress = call.CallerAddress,
                StartTime = _dal.Config.Clock,
                MaxEndTime = call.MaxEndTime,
                Latitude = latitude ?? 0,
                Longitude = longitude ?? 0,
                Status = DO.Enums.CallStatusEnum.New
            };
            _dal.Call.Create(newCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BlAlreadyExistsException($"A call with ID {call.Id} already exists.", ex);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to create call.", ex);
        }
    }
    public List<ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, CallType? filterByType = null, ClosureType? sortByClosureType = null)
    {
        try
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
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to get closed calls of volunteer.", ex);
        }
    }
    public List<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? filterByType = null, OpenCallSortField? sortByField = null)
    {
        try
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
                    DistanceFromVolunteer = Helpers.CallManager.CalculateDistance(volunteer.Longitude, volunteer.Latitude, call.Longitude, call.Latitude)
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
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to get open calls for volunteer.", ex);
        }
    }
    public void UpdateEndTreatment(int id, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
                throw new BlDoesNotExistException($"Assignment with ID {assignmentId} not found.");
            var call = _dal.Call.Read(assignment.CallId);
            if (call == null)
                throw new BlDoesNotExistException($"Call with ID {assignment.CallId} not found.");
            if (assignment.VolunteerId != id)
                throw new BlInvalidException($"Assignment {assignmentId} does not belong to volunteer {id}.");
            if (assignment.EndTime != null && assignment.EndStatus != null)
                throw new BlInvalidException($"Assignment {assignmentId} has already been finished.");
            var assignmentToUpdate = new DO.Assignment
            {
                Id = assignment.Id,
                VolunteerId = assignment.VolunteerId,
                CallId = assignment.CallId,
                ArrivalTime = assignment.ArrivalTime,
                EndTime = _dal.Config.Clock,
                EndStatus = TerminationTypeEnum.Treated
            };
            _dal.Assignment.Update(assignmentToUpdate);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to update end treatment.", ex);
        }
    }
    public void CancelAssignmentTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId);
            var volunteer = _dal.Volunteer.Read(volunteerId);
            if (assignment == null)
                throw new BlDoesNotExistException($"Assignment with ID {assignmentId} not found.");
            if (assignment.VolunteerId != volunteerId || volunteer.Role != RoleEnum.Manager)
                throw new BlInvalidException("You do not have permission to perform this action.");
            if (assignment.EndTime != null && assignment.EndStatus != null)
                throw new BlInvalidException($"Assignment {assignmentId} is not open or in progress, cannot cancel.");
            var endStatusToUpdate = volunteer.Role == RoleEnum.Manager ? TerminationTypeEnum.ManagerCancelled : TerminationTypeEnum.SelfCancelled;
            var assignmentToUpdate = new DO.Assignment
            {
                Id = assignment.Id,
                VolunteerId = assignment.VolunteerId,
                CallId = assignment.CallId,
                ArrivalTime = assignment.ArrivalTime,
                EndTime = _dal.Config.Clock,
                EndStatus = endStatusToUpdate
            };
            _dal.Assignment.Update(assignmentToUpdate);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to cancel assignment treatment.", ex);
        }
    }
    public void RequestAssignmentTreatment(int voluneetId, int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId);
            var existingAssignment = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId)
                .ToList();
            if (existingAssignment != null || call.Status == CallStatusEnum.InProgress || call.MaxEndTime < _dal.Config.Clock)
            {
                throw new BlInvalidException($"Call {callId} already has an expired or in-progress assignment.");
            }
            var assignmentToAdd = new DO.Assignment
            {
                VolunteerId = voluneetId,
                CallId = callId,
                ArrivalTime = _dal.Config.Clock,
                EndTime = null,
                EndStatus = null
            };
            _dal.Assignment.Create(assignmentToAdd);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to request assignment treatment.", ex);
        }
    }
    public BO.Call Read(int id)
    {
        try
        {
            var readCall = _dal.Call.Read(id);
            return Helpers.CallManager.ConvertDoToBo(readCall);
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException("Call read did not succeeded /n", ex);
        }
    }
}

#region לפני זריקת חריגות
//using BO;
//using BlApi;
//using DO;
//using DalApi;
//using Helpers;
//using static DO.Enums;

//namespace BlImplementation;
//internal class CallImplementation : BlApi.ICall
//{
//    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
//    public int[] GetCallStatusCounts()
//    {
//        var calls = _dal.Call.ReadAll();
//        return calls
//            .GroupBy(c => (int)c.Status)
//            .ToDictionary(g => g.Key, g => g.Count())
//            .Select(kvp => kvp.Value)
//            .ToArray();
//    }
//    public void CloseExpiredCalls()
//    {
//        var assignmentsData = _dal.Assignment.ReadAll();
//        var allCalls = _dal.Call.ReadAll();
//        var expiredCalls = allCalls
//            .Where(c => c.MaxEndTime < _dal.Config.Clock && c.Status != CallStatusEnum.Closed)
//            .ToList();
//        foreach (var call in expiredCalls)
//        {
//            var assignment = assignmentsData.FirstOrDefault(a => a.CallId == call.Id);
//            if (assignment == null)
//            {
//                var newAssignment = new DO.Assignment
//                {
//                    VolunteerId = 0,
//                    CallId = call.Id,
//                    ArrivalTime = null,
//                    EndTime = _dal.Config.Clock,
//                    EndStatus = Enums.TerminationTypeEnum.Expired
//                };
//                _dal.Assignment.Create(newAssignment);
//            }
//            else if (assignment.EndTime == null)
//            {
//                var updatedAssignment = new DO.Assignment
//                (
//                    assignment.Id,
//                    assignment.VolunteerId,
//                    assignment.CallId,
//                    assignment.ArrivalTime,
//                    _dal.Config.Clock
//                );
//                _dal.Assignment.Update(updatedAssignment);
//            }
//            _dal.Call.Update(new DO.Call
//            {
//                Id = call.Id,
//                Status = CallStatusEnum.Expired,
//                Type = call.Type,
//                Description = call.Description,
//                CallerAddress = call.CallerAddress,
//                Latitude = call.Latitude,
//                Longitude = call.Longitude,
//                StartTime = call.StartTime,
//                MaxEndTime = call.MaxEndTime
//            });
//        }
//    }
//    //public IEnumerable<CallInList> GetCallsList(CallFieldFilter? filterBy, object? filterValue, CallFieldFilter? sortBy)
//    //{
//    //    var calls = _dal.Call.ReadAll();
//    //    if (filterBy != null && filterValue != null)
//    //    {
//    //        calls = calls.Where(c => Helpers.CallManager.GetFieldValue(c, filterBy.Value).Equals(filterValue));
//    //    }
//    //    if (sortBy != null)
//    //    {
//    //        calls = calls.OrderBy(c => Helpers.CallManager.GetFieldValue(c, sortBy.Value));
//    //    }
//    //    else
//    //    {
//    //        calls = calls.OrderBy(c => c.Id);
//    //    }
//    //    return calls;
//    //}
//    public BO.CallAssignInList GetCallDetails(int callId)
//    {
//        try
//        {
//            var callData = _dal.Call.Read(callId);
//            var assignmentsData = _dal.Assignment.Read(callId);
//            var volunteerData = _dal.Volunteer.Read(assignmentsData.VolunteerId);
//            return new BO.CallAssignInList
//            {
//                VolunteerId = assignmentsData.VolunteerId,
//                VolunteerName = volunteerData.FullName,
//                StartTreatmentTime = assignmentsData.ArrivalTime,
//                EndTreatmentTime = assignmentsData.EndTime,
//                EndType = (AssignmentEndType)assignmentsData.EndStatus
//            };
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("CallAssignInList not found", ex);
//        }
//    }
//    public void UpdateCall(BO.Call call)
//    {
//        try
//        {
//            var existingCall = _dal.Call.Read(call.Id);
//            string checkValues = Helpers.CallManager.IsValid(call);
//            if (checkValues == "true")
//            {
//                var newCall = new DO.Call
//                {
//                    Id = call.Id,
//                    Type = (DO.Enums.CallTypeEnum)call.Type,
//                    Description = call.Description,
//                    CallerAddress = call.CallerAddress,
//                    Latitude = CallManager.GetLatitudLongitute(call.CallerAddress).Latitude, // Get latitude for the address
//                    Longitude = CallManager.GetLatitudLongitute(call.CallerAddress).Longitude, // Get longitude for the address
//                    StartTime = call.StartTime,
//                    MaxEndTime = call.MaxEndTime
//                };
//                _dal.Call.Update(newCall);
//            }
//            else
//                throw new Exception(checkValues + " - this field is not valid");
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("ERROR" + ex);
//        }
//    }
//    public void DeleteCall(int callId)
//    {
//        DO.Call call;
//        try
//        {
//            call = _dal.Call.Read(callId);
//        }
//        catch (Exception ex)
//        {
//            throw new KeyNotFoundException($"No call found with ID {callId}.", ex);
//        }
//        if (call.Status != CallStatusEnum.Open)
//            throw new InvalidOperationException("Only calls with status 'Open' can be deleted.");
//        bool hasAssignment = _dal.Assignment.ReadAll()
//            .Any(a => a.CallId == callId);
//        if (hasAssignment)
//            throw new InvalidOperationException("Cannot delete a call that was assigned to a volunteer.");
//        _dal.Call.Delete(callId);
//    }
//    public void CreateCall(BO.Call call)
//    {
//        string validationResult = Helpers.CallManager.IsValid(call);
//        if (validationResult != "true")
//            throw new ArgumentException($"Field '{validationResult}' is invalid.");
//        try
//        {
//            DO.Call newCall = new DO.Call
//            {
//                Id = call.Id,
//                Description = call.Description,
//                CallerAddress = call.CallerAddress,
//                StartTime = call.StartTime,
//                MaxEndTime = call.MaxEndTime,
//                Latitude = call.Latitude ?? 0,
//                Longitude = call.Longitude ?? 0,
//                Status = DO.Enums.CallStatusEnum.Open
//            };
//            _dal.Call.Create(newCall);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception($"A call with ID {call.Id} already exists.", ex);
//        }
//    }
//    public List<ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, CallType? filterByType = null, ClosureType? sortByClosureType = null)
//    {
//        var allClosedCalls = _dal.Call.ReadAll()
//            .Where(call =>
//                call.Status == DO.Enums.CallStatusEnum.Closed &&
//                _dal.Assignment.Read(call.Id).VolunteerId == volunteerId)
//            .Select(call => Helpers.CallManager.ConvertToClosedCallInList(call));
//        if (filterByType.HasValue)
//        {
//            allClosedCalls = allClosedCalls
//                .Where(call => call.CallType == filterByType.Value);
//        }
//        if (sortByClosureType.HasValue)
//        {
//            allClosedCalls = allClosedCalls
//                .OrderBy(call => call.ClosureType.HasValue ? call.ClosureType.Value : ClosureType.Treated);
//        }
//        else
//        {
//            allClosedCalls = allClosedCalls
//                .OrderBy(call => call.Id);
//        }
//        return allClosedCalls.ToList();
//    }
//    public List<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? filterByType = null, OpenCallSortField? sortByField = null)
//    {
//        var volunteer = _dal.Volunteer.Read(volunteerId);
//        var openCalls = _dal.Call.ReadAll()
//            .Where(call =>
//                call.Status == DO.Enums.CallStatusEnum.Open ||
//                call.Status == DO.Enums.CallStatusEnum.Aborted)
//            .Select(call => new OpenCallInList
//            {
//                Id = call.Id,
//                CallType = (CallType)call.Type,
//                Description = call.Description,
//                Address = call.CallerAddress,
//                OpeningTime = call.StartTime,
//                MaxFinishTime = call.MaxEndTime,
//                DistanceFromVolunteer = Helpers.CallManager.CalculateDistance(volunteer.Longitude, volunteer.Latitude, call.Longitude, call.Latitude)
//            });
//        if (filterByType.HasValue)
//        {
//            openCalls = openCalls.Where(call => call.CallType == filterByType.Value);
//        }
//        if (sortByField.HasValue)
//        {
//            openCalls = sortByField.Value switch
//            {
//                OpenCallSortField.Id => openCalls.OrderBy(call => call.Id),
//                OpenCallSortField.OpeningTime => openCalls.OrderBy(call => call.OpeningTime),
//                OpenCallSortField.MaxFinishTime => openCalls.OrderBy(call => call.MaxFinishTime),
//                OpenCallSortField.DistanceFromVolunteer => openCalls.OrderBy(call => call.DistanceFromVolunteer),
//                _ => openCalls
//            };
//        }
//        else
//        {
//            openCalls = openCalls.OrderBy(call => call.Id);
//        }
//        return openCalls.ToList();
//    }
//    public void UpdateEndTreatment(int id, int assignmentId)
//    {
//        try
//        {
//            var assignment = _dal.Assignment.Read(assignmentId);
//            if (assignment == null)
//                throw new Exception($"Assignment with ID {assignmentId} not found.");
//            var call = _dal.Call.Read(assignment.CallId);
//            if (call == null)
//                throw new Exception($"Call with ID {assignment.CallId} not found.");
//            if (assignment.VolunteerId != id)
//                throw new Exception($"Assignment {assignmentId} does not belong to volunteer {id}.");
//            if (assignment.EndTime != null && assignment.EndStatus != null)
//                throw new Exception($"Assignment {assignmentId} has already been finished.");
//            var assignmentToUpdate = new DO.Assignment
//            {
//                Id = assignment.Id,
//                VolunteerId = assignment.VolunteerId,
//                CallId = assignment.CallId,
//                ArrivalTime = assignment.ArrivalTime,
//                EndTime = _dal.Config.Clock,
//                EndStatus = TerminationTypeEnum.Treated
//            };
//            _dal.Assignment.Update(assignmentToUpdate);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("ERROR", ex);
//        }

//    }
//    public void CancelAssignmentTreatment(int volunteerId, int assignmentId)
//    {
//        try
//        {
//            var assignment = _dal.Assignment.Read(assignmentId);
//            var volunteer = _dal.Volunteer.Read(volunteerId);
//            if (assignment == null)
//                throw new Exception($"Assignment with ID {assignmentId} not found.");
//            if (assignment.VolunteerId != volunteerId || volunteer.Role != RoleEnum.Mentor)
//                throw new Exception("You do not have permission to perform this action.");
//            if (assignment.EndTime != null && assignment.EndStatus != null)
//                throw new Exception($"Assignment {assignmentId} is not open or in progress, cannot cancel.");
//            var endStatusToUpdate = volunteer.Role == RoleEnum.Mentor ? TerminationTypeEnum.Canceled : TerminationTypeEnum.SelfCanceled;
//            var assignmentToUpdate = new DO.Assignment
//            {
//                Id = assignment.Id,
//                VolunteerId = assignment.VolunteerId,
//                CallId = assignment.CallId,
//                ArrivalTime = assignment.ArrivalTime,
//                EndTime = _dal.Config.Clock,
//                EndStatus = endStatusToUpdate
//            };
//            _dal.Assignment.Update(assignmentToUpdate);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("ERROR", ex);
//        }
//    }
//    public void RequestAssignmentTreatment(int voluneetId, int callId)
//    {
//        try
//        {
//            var call = _dal.Call.Read(callId);
//            // שלב 1: בדיקה אם קיימת כבר האצקה פתוחה לקריאה הזו
//            var existingAssignment = _dal.Assignment.ReadAll()
//                .Where(a => a.CallId == callId)
//                .ToList();
//            if (existingAssignment != null || call.Status == CallStatusEnum.InProgress || call.MaxEndTime < _dal.Config.Clock)
//            {
//                throw new Exception($"Call {callId} already has an expired or in-progress assignment.");
//            }
//            var assignmentToAdd = new DO.Assignment
//            {
//                VolunteerId = voluneetId,
//                CallId = callId,
//                ArrivalTime = _dal.Config.Clock,
//                EndTime = null,
//                EndStatus = null
//            };
//            _dal.Assignment.Create(assignmentToAdd);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("ERROR", ex);
//        };
//    }
//}
#endregion
