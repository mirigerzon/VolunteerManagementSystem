using System;
using BO;
using System.Collections.Generic;

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
public interface ICall
{
    int[] GetCallStatusCounts();
    void CloseExpiredCalls();
    IEnumerable<BO.CallInList> GetCallsList(CallFieldFilter? filterBy = null, object? filterValue = null, CallFieldFilter? sortBy = null);
    BO.Call Read(int id);
    void UpdateCall(BO.Call call);
    void Delete(int id);
    void Create(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, BO.CallStatus? statusFilter = null, BO.CallSortField? sortBy = null);
}
