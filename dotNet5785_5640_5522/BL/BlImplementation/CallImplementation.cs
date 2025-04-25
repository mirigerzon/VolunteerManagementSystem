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
    public void UpdateExpiredCalls()
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
        var existingCall = _dal.Call.Read(call.Id);
        string checkValues = Helpers.CallManager.IsValid(call);
        if (checkValues == "true")
        {
            //לעדכן את מתנדב
            var volunteerToUpdate = Helpers.VolunteerManager.ConvertBoToDo(volunteer);
            _dal.Volunteer.Create(volunteerToUpdate);
        }
        else
            throw new Exception(checkValues + " - this field is not valid");
        //var assignment = _dal.Assignment
        //    .ReadAll()
        //    .FirstOrDefault(a => a.CallId == call.Id);
        //if (assignment != null)
        //{
        //    int volunteerId = assignment.VolunteerId;
        //    try
        //    {
        //        var volunteer = _dal.Volunteer.Read(volunteerId);
        //    }
        //    catch
        //    {
        //        throw new InvalidOperationException($"Volunteer with ID {volunteerId} does not exist.");
        //    }
        //}
        var updatedCall = new DO.Call
        {
            Id = call.Id,
            Type = call.Type,
            Status = call.Status,
            Description = call.Description,
            CallerAddress = call.CallerAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            StartTime = call.StartTime,
            MaxEndTime = call.MaxEndTime
        };
        _dal.Call.Update(updatedCall);
    }
    public void CloseExpiredCalls()
    {
        throw new NotImplementedException();
    }
    public IEnumerable<CallInList> GetCallsList(CallFieldFilter? filterBy = null, object? filterValue = null, CallFieldFilter? sortBy = null)
    {
        throw new NotImplementedException();
    }
    public BO.Call Read(int id)
    {
        throw new NotImplementedException();
    }
    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
    public void Create(BO.Call call)
    {
        throw new NotImplementedException();
    }
    public IEnumerable<ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, CallStatus? statusFilter = null, CallSortField? sortBy = null)
    {
        throw new NotImplementedException();
    }
}



//using BlApi;
//using Helpers;
//namespace BlImplementation;

//internal class CallImplementation : ICall
//{
//    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
//    public void AddCall(BO.Call newCall)
//    {
//        try
//        {
//            // Validate data and convert to DO.Call using shared helper
//            DO.Call callToCreate = Helpers.CallManager.ValidateAndConvertToDO(newCall);

//            // Add call to the data layer
//            _dal.Call.Create(callToCreate);
//        }
//        catch (DO.DalAlreadyExistsException ex)
//        {
//            throw new($"Call with ID {newCall.Id} already exists.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new("An error occurred while adding the new call.", ex);
//        }
//    }
//    public void AssignCallToVolunteer(int volunteerId, int callId)
//    {
//        try
//        {
//            // Step 1: Retrieve the call from the data layer
//            DO.Call call = _dal.Call.Read(callId)
//                ?? throw new Exception($"Call with ID {callId} not found.");

//            // Step 2: Check if the call has already expired
//            DateTime now = Helpers.ClockManager.Now;
//            if (call.ClosingTime != null && now > call.ClosingTime)
//                throw new Exception("This call has already expired and cannot be assigned.");

//            // Step 3: Check if the call is already being handled (an open assignment exists)
//            var existingAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId);
//            bool isAlreadyInProgress = existingAssignments.Any(a => a.TreatmentEndTime == null);
//            if (isAlreadyInProgress)
//                throw new Exception("This call is already being handled by another volunteer.");

//            // Step 4: Create a new assignment with the volunteer ID and call ID
//            DO.Assignment newAssignment = new DO.Assignment
//            {
//                CallId = callId,
//                VolunteerId = volunteerId,
//                TreatmentEntryTime = now,
//                TreatmentEndTime = null,
//                TypeOfTreatmentTermination = null
//            };

//            // Step 5: Add the assignment to the data layer
//            _dal.Assignment.Create(newAssignment);
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            // If either the call or volunteer doesn't exist, rethrow with higher-level exception
//            throw new Exception("Call or volunteer not found in the database.", ex);
//        }
//        catch (Exception ex)
//        {
//            // Catch-all for any unexpected error
//            throw new Exception("Failed to assign the call to the volunteer.", ex);
//        }
//    }
//    public void CancelCallHandling(int requesterId, int assignmentId)
//    {
//        try
//        {
//            // שלב 1: שליפת ההקצאה
//            DO.Assignment assignment = _dal.Assignment.Read(assignmentId)
//                ?? throw new Exception($"Assignment with ID {assignmentId} does not exist.");

//            // שלב 2: שליפת פרטי המבקש
//            DO.Volunteer requester = _dal.Volunteer.Read(requesterId)
//                ?? throw new Exception($"Requester with ID {requesterId} does not exist.");

//            // שלב 3: בדיקת הרשאה - מותר רק אם הוא המתנדב שהוקצה או אם הוא מנהל
//            bool isManager = requester.Position == DO.Volunteer.PositionEnum.manager;
//            bool isOwner = assignment.VolunteerId == requesterId;

//            if (!isManager && !isOwner)
//                throw new Exception("Unauthorized: Only the assigned volunteer or a manager can cancel this assignment.");

//            // שלב 4: בדיקה אם כבר הסתיים או בוטל
//            if (assignment.TreatmentEndTime != null || assignment.TypeOfTreatmentTermination != null)
//                throw new Exception("This assignment is already completed or cancelled.");

//            // שלב 5: עדכון שדות
//            assignment.TreatmentEndTime = Helpers.ClockManager.Now;
//            assignment.TypeOfTreatmentTermination = isOwner
//                ? DO.Assignment.TypeOfTreatmentTerminationEnum.SelfCanceled
//                : DO.Assignment.TypeOfTreatmentTerminationEnum.ManagerCanceled;

//            // שלב 6: עדכון בשכבת הנתונים
//            _dal.Assignment.Update(assignment);
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new Exception($"Assignment or volunteer not found in the database.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("An error occurred while attempting to cancel the assignment.", ex);
//        }
//    }
//    public void DeleteCallDetails(int callId)
//    {
//        try
//        {
//            // Retrieve the call from the data layer.
//            // If the call does not exist, the DAL will throw DalDoesNotExistException.
//            DO.Call doCall = _dal.Call.Read(callId);

//            // Convert the DO.Call to a BO.Call to evaluate status and assignments
//            BO.Call boCall = Helpers.CallManager.ToBO(doCall);

//            // Only calls with status "Open" are allowed to be deleted
//            if (boCall.Status != BO.CallStatus.Open)
//                throw new("Only calls in 'Open' status can be deleted.");

//            // If the call was ever assigned to any volunteer, it cannot be deleted
//            if (boCall.Assignments != null && boCall.Assignments.Any())
//                throw new("Cannot delete a call that was already assigned to a volunteer.");

//            // If all conditions are met, proceed with deletion
//            _dal.Call.Delete(callId);
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            // Re-throw a higher-level exception for the presentation layer
//            throw new($"Call with ID {callId} does not exist in the system.", ex);
//        }
//        catch (Exception ex)
//        {
//            // Catch-all for any unexpected errors during the process
//            throw new("An unexpected error occurred while attempting to delete the call.", ex);
//        }
//    }
//    public void FinishCallHandling(int volunteerId, int assignmentId)
//    {
//        try
//        {
//            // שלב 1: שליפת ההקצאה לפי מזהה
//            DO.Assignment assignment = _dal.Assignment.Read(assignmentId)
//                ?? throw new Exception($"Assignment with ID {assignmentId} does not exist.");

//            // שלב 2: בדיקת הרשאה - האם המתנדב המבצע הוא זה שרשום על ההקצאה
//            if (assignment.VolunteerId != volunteerId)
//                throw new Exception("Unauthorized: This assignment does not belong to the requesting volunteer.");

//            // שלב 3: בדיקה האם ההקצאה פתוחה (לא הסתיימה, לא בוטלה, לא פגה)
//            if (assignment.TreatmentEndTime != null || assignment.TypeOfTreatmentTermination != null)
//                throw new Exception("This assignment has already been closed or cancelled.");

//            // שלב 4: עדכון זמן סיום וסוג סיום הטיפול
//            assignment.TreatmentEndTime = Helpers.ClockManager.Now;
//            assignment.TypeOfTreatmentTermination = DO.Assignment.TypeOfTreatmentTerminationEnum.Handled;

//            // שלב 5: שמירת ההקצאה לאחר העדכון
//            _dal.Assignment.Update(assignment);
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new Exception($"Assignment with ID {assignmentId} not found in the database.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("An error occurred while trying to finish call handling.", ex);
//        }
//    }
//    public int[] GetCallCounts()
//    {
//        try
//        {
//            var statusCount = Enum.GetValues(typeof(BO.CallStatus)).Length;

//            // Return the counts as an array, where each index i represents the count of status i
//            return _dal.Call
//                .ReadAll()
//                .Select(Helpers.CallManager.ToBO)
//                .GroupBy(call => (int)call.Status)
//                .Aggregate(
//                    new int[statusCount],
//                    (acc, group) =>
//                    {
//                        acc[group.Key] = group.Count();
//                        return acc;
//                    }
//                );
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new("Error accessing calls from database", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new("Unexpected error occurred during status count", ex);
//        }
//    }
//    public BO.Call GetCallDetails(int callId)
//    {
//        try
//        {
//            // קבלת הקריאה משכבת הנתונים
//            var doCall = _dal.Call.Read(callId);
//            if (doCall == null)
//                throw new($"Call with ID {callId} not found.");

//            // המרת הקריאה ל-BO כולל ההקצאות והסטטוס
//            var boCall = Helpers.CallManager.ToBO(doCall);

//            return boCall;
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new($"Call with ID {callId} not found in the database.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new("An unexpected error occurred while retrieving call details.", ex);
//        }
//    }
//    public IEnumerable<BO.CallInList> GetCallList(BO.CallTypeEnum? filterField, object? filterValue, BO.CallTypeEnum? sortField)
//    {
//        DateTime now = Helpers.ClockManager.Now;

//        var calls = _dal.Call
//            .ReadAll()
//            .Select(Helpers.CallManager.ToBO)
//            .Select(call =>
//            {
//                // שליפת ההקצאה האחרונה
//                var latestAssignment = call.Assignments?
//                    .OrderByDescending(a => a.StartTreatment)
//                    .FirstOrDefault();

//                // חישוב משך טיפול אם הסתיים
//                TimeSpan? duration = (latestAssignment?.EndTreatment != null)
//                    ? latestAssignment.EndTreatment - call.OpenedAt
//                    : null;

//                // חישוב זמן נותר אם יש מקסימום זמן
//                TimeSpan? remaining = call.MaxEndTime != null
//                    ? call.MaxEndTime - now
//                    : null;

//                return new BO.CallInList
//                {
//                    Id = latestAssignment?.VolunteerId,
//                    CallId = call.Id,
//                    CallType = call.CallType,
//                    OpenedAt = call.OpenedAt,
//                    RemainingTime = remaining,
//                    LastVolunteerName = latestAssignment?.VolunteerName,
//                    Duration = duration,
//                    Status = call.Status,
//                    AssignmentCount = call.Assignments?.Count ?? 0
//                };
//            });

//        // סינון לפי סוג קריאה בלבד (אם filterField אינו null)
//        if (filterField != null)
//        {
//            calls = calls.Where(c => c.CallType.Equals(filterValue));
//        }

//        // מיון לפי סוג קריאה אם צריך, אחרת לפי CallId
//        if (sortField != null)
//        {
//            calls = calls.OrderBy(c => c.CallType);
//        }
//        else
//        {
//            calls = calls.OrderBy(c => c.CallId);
//        }

//        return calls;
//    }
//    public IEnumerable<BO.ClosedCallInList> GetClosedCallsHandledByVolunteer(
//        int volunteerId,
//        BO.CallTypeEnum? filter,
//        BO.ClosedCallInList? sortField)
//    {
//        try
//        {
//            // Step 1: Retrieve all assignments of the volunteer
//            var assignments = _dal.Assignment
//                .ReadAll(a => a.VolunteerId == volunteerId);

//            // Step 2: Filter to include only those whose associated call status is Closed
//            var closedCalls = assignments
//                .Select(a =>
//                {
//                    var call = _dal.Call.Read(a.CallId);
//                    var callBO = Helpers.CallManager.ToBO(call); // for status calculation

//                    // Only include if the calculated status is Closed
//                    if (callBO.Status != BO.CallStatus.Closed)
//                        return null;

//                    return new BO.ClosedCallInList
//                    {
//                        Id = call.Id,
//                        CallType = (BO.CallTypeEnum)call.Type,
//                        FullAddress = call.Address,
//                        OpenedAt = call.OpeningTime,
//                        EnteredAt = a.TreatmentEntryTime,
//                        FinishedAt = a.TreatmentEndTime,
//                        FinishType = a.TypeOfTreatmentTermination != null
//                            ? (BO.FinishTypeEnum?)a.TypeOfTreatmentTermination
//                            : null
//                    };
//                })
//                .Where(c => c != null)!; // Filter out nulls

//            // Step 3: Filter by call type if provided
//            if (filter != null)
//                closedCalls = closedCalls.Where(c => c.CallType == filter);

//            // Step 4: Sort by the selected field
//            closedCalls = sortField switch
//            {
//                BO.ClosedCallInList.CallType => closedCalls.OrderBy(c => c.CallType),
//                BO.ClosedCallInList.OpenedAt => closedCalls.OrderBy(c => c.OpenedAt),
//                BO.ClosedCallInList.EnteredAt => closedCalls.OrderBy(c => c.EnteredAt),
//                BO.ClosedCallInList.FinishedAt => closedCalls.OrderBy(c => c.FinishedAt),
//                BO.ClosedCallInList.FinishType => closedCalls.OrderBy(c => c.FinishType),
//                _ => closedCalls.OrderBy(c => c.Id)
//            };

//            return closedCalls;
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new Exception("Volunteer or call record not found.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("An error occurred while retrieving closed calls.", ex);
//        }
//    }
//    public IEnumerable<BO.OpenCallInList> GetOpenCallsHandledByVolunteer(
//    int volunteerId,
//    BO.CallType? filter,
//    BO.OpenCallInList? sortField)
//    {
//        try
//        {
//            // Step 1: Retrieve the volunteer's location
//            var volunteer = _dal.Volunteer.Read(volunteerId);
//            if (volunteer.Latitude == null || volunteer.Longitude == null)
//                throw new("Volunteer does not have location coordinates.");

//            // Step 2: Retrieve all open/at-risk calls
//            var openCalls = _dal.Call.ReadAll()
//                .Where(call =>
//                {
//                    var boCall = Helpers.CallManager.ToBO(call);
//                    return boCall.Status == BO.CallStatus.Open || boCall.Status == BO.CallStatus.OpenAtRisk;
//                });

//            // Step 3: Filter by call type if needed
//            if (filter != null)
//                openCalls = openCalls.Where(c => c.Type == (DO.Call.TYPE)filter);

//            // Step 4: Project to OpenCallInList with distance calculation
//            var result = openCalls.Select(call => new BO.OpenCallInList
//            {
//                Id = call.Id,
//                CallType = (BO.CallType)call.Type,
//                Description = call.Description,
//                Address = call.Address,
//                Start = call.OpeningTime,
//                MaxEndTime = call.ClosingTime,
//                Distance = Helpers.Tools.CalculateDistanceBetween(volunteer, call)
//            });

//            // Step 5: Apply sorting
//            result = sortField switch
//            {
//                BO.OpenCallInList.CallType => result.OrderBy(c => c.CallType),
//                BO.OpenCallInList.Start => result.OrderBy(c => c.Start),
//                BO.OpenCallInList.MaxEndTime => result.OrderBy(c => c.MaxEndTime),
//                BO.OpenCallInList.Distance => result.OrderBy(c => c.Distance),
//                _ => result.OrderBy(c => c.Id)
//            };

//            return result;
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new("Volunteer or call does not exist.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new("An error occurred while retrieving open calls.", ex);
//        }
//    }
//    public void UpdateCallDetails(BO.Call callToUpdate)
//    {
//        try
//        {
//            // Use shared helper to validate logic and format, and convert to DO.Call
//            DO.Call updatedCall = Helpers.CallManager.ValidateAndConvertToDO(callToUpdate)
//            // Attempt to update in data layer
//            _dal.Call.Update(updatedCall);
//        }
//        catch (DO.DalDoesNotExistException ex)
//        {
//            throw new($"Call with ID {callToUpdate.Id} does not exist in the system.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new("An error occurred while updating the call details.", ex);
//        }
//    }


//}