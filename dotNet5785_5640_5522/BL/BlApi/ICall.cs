using System;
using BO;
using System.Collections.Generic;
using Helpers;

namespace BlApi;

/// <summary>
/// Interface for managing service calls.
/// Includes:
/// - GetCallStatusCounts: returns count of calls per status.
/// - CloseExpiredCalls: updates open calls that have expired.
/// - GetCallsList: returns a list of calls with optional filter and sort.
/// - Read: retrieves full details of a specific call, including its assignments.
/// - Update: updates an existing call.
/// - Delete: deletes a call (only if it's open and unassigned).
/// - Create: adds a new call.
/// - GetClosedCallsForVolunteer: returns closed calls handled by a specific volunteer, with optional filters and sorting.
/// </summary>
public interface ICall : IObservable
{
    int[] GetCallStatusCounts();
    void CloseExpiredCalls();
    IEnumerable<BO.CallInList> GetCallsList(CallFieldFilter? filterBy = null, object? filterValue = null, CallFieldFilter? sortBy = null);
    BO.Call GetCallDetails(int callId);
    Task Update(BO.Call call);
    void Delete(int id);
    void Create(BO.Call call);
    BO.Call Read(int id);
    List<ClosedCallInList> GetClosedCallsOfVolunteer(int volunteerId, CallType? filterByType = null, TerminationTypeEnum? sortByClosureType = null);
    List<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? filterByType = null, OpenCallSortField? sortByField = null);
    void UpdateEndTreatment(int id, int assignmentId);
    void CancelAssignmentTreatment(int volunteerId, int? assignmentId);
    CallInProgress RequestAssignmentTreatment(int voluneetId, int callId);
}
