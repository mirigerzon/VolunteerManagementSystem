using BO;
using BlApi;
using DO;
using DalApi;
using Helpers;

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
    public IEnumerable<CallInList> GetCallsList(CallFieldFilter? filterBy, object? filterValue, CallFieldFilter? sortBy)
    {
        var calls = _dal.Call.ReadAll();
        if (filterBy != null && filterValue != null)
        {
            calls = calls.Where(c => Helpers.callManager.GetFieldValue(c, filterBy.Value).Equals(filterValue));
        }
        if (sortBy != null)
        {
            calls = calls.OrderBy(c => Helpers.callManager.GetFieldValue(c, sortBy.Value));
        }
        else
        {
            calls = calls.OrderBy(c => c.Id);
        }
        return calls;
    }

    public void CloseExpiredCalls()
    {
        throw new NotImplementedException();
    }

    public void Create(BO.Call call)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, CallStatus? statusFilter = null, CallSortField? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public BO.Call Read(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> ReadList(CallFieldFilter? filterBy = null, object? filterValue = null, CallSortField? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public void Update(BO.Call call)
    {
        throw new NotImplementedException();
    }
}
