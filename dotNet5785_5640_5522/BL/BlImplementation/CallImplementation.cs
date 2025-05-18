using BO;
using BlApi;
using DO;
using DalApi;
using Helpers;
using static DO.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Buffers.Text;
using System;

namespace BlImplementation;
internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    //This method retrieves the count of calls grouped by their status(e.g., open, closed, expired) by reading all calls from the data layer and grouping them based on their status.
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
    //This method identifies expired calls (calls whose maximum end time has passed) and updates them to reflect their expiration. It also creates or updates assignments for those expired calls.
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
    //This method provides a list of calls, with optional filters and sorting by various fields such as call ID, type, status, start time, and maximum end time.
    public IEnumerable<CallInList> GetCallsList(CallFieldFilter? filterBy = null, object? filterValue = null, CallFieldFilter? sortBy = null)
    {
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
    //This method fetches detailed information about a specific call by its ID, including assignment information for volunteers handling the call.
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
    //This method allows updating an existing call if it is valid according to predefined validation rules.
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
    //This method deletes a call, but only if its status is 'Open' and it has no associated assignments.
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
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException($"No call found with ID {callId}.", ex);
        }
    }
    //This method creates a new call after validating the input and setting coordinates based on the caller's address.
    public void Create(BO.Call call)
    {
        string validationResult = Helpers.CallManager.IsValid(call);
        if (validationResult != "true")
            throw new BlInvalidException($"Field '{validationResult}' is invalid.");
        try
        {
            double? latitude= Helpers.CallManager.GetCoordinates(call.CallerAddress)[0];
            double? longitude= Helpers.CallManager.GetCoordinates(call.CallerAddress)[1];

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
            CallManager.Observers.NotifyListUpdated();
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
    //This method retrieves a list of closed calls assigned to a specific volunteer, with optional filtering and sorting.
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
    //This method provides a list of open or aborted calls for a volunteer, with sorting by various fields like opening time, max finish time, and distance from the volunteer.
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
                openCalls = openCalls.OrderBy(call => call.Id); //stage 5
            }
            return openCalls.ToList();
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to get open calls for volunteer.", ex);
        }
    }
    //This method updates the end treatment status for an assignment, marking it as "treated" once the volunteer finishes their treatment.
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
            CallManager.Observers.NotifyListUpdated(); //stage 5
            CallManager.Observers.NotifyItemUpdated(call.Id); //stage 5
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to update end treatment.", ex);
        }
    }
    //This method allows a volunteer (or manager) to cancel an assignment, marking it as canceled either by the volunteer or by the manager.
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
    //This method handle the creation of an assignment for a volunteer to a service call. 
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
    //This method allows read an existing call by id.
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
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}