namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Net;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        Call newCall = new Call(
            item.Id,
            item.Status,
            item.Type,
            item.Description,
            item.CallerAddress,
            item.Latitude,
            item.Longitude,
            item.StartTime,
            item.MaxEndTime
        );
        Call? call = DataSource.Calls.FirstOrDefault(x => x.Id == item.Id);
        if (call != null)
            throw new DalDoesNotExistException("Volunteer with this ID does exist.");
        DataSource.Calls.Add(newCall);
    }
    public void Delete(int id)
    {
        var callToRemove = DataSource.Calls.Find(x => x.Id == id);
        if (callToRemove != null)
            DataSource.Calls.Remove(callToRemove);
        else
            throw new DalDoesNotExistException("Call with this ID does not exist.");
    }
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
        Console.WriteLine("\n There are no calls in Call list");
    }
    public Call? Read(int id)
    {
        Call? call = DataSource.Calls.FirstOrDefault(item => item.Id == id);
        if (call == null)
            throw new DalDoesNotExistException($"Call with ID {id} does not exist.");
        return call;
    }
    public Call? Read(Func<Call, bool> filter)
    {
        Call? call = DataSource.Calls.FirstOrDefault(filter);
        if (call == null)
            throw new DalDoesNotExistException("No call matches the provided filter.");
        return call;
    }
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        IEnumerable<Call> results = filter != null
            ? from item in DataSource.Calls where filter(item) select item
            : DataSource.Calls;
        if (!results.Any())
            Console.WriteLine("No calls found matching the filter.");
        return results;
    }
    public void Update(Call item)
    {
        var existingCall = DataSource.Assignments.FindIndex(x => x.Id == item.Id);
        if (existingCall != -1)
        {
            DataSource.Calls[existingCall] = new Call(
                item.Id,
                item.Type,
                item.Description,
                item.CallerAddress,
                item.Latitude,
                item.Longitude
            )
            {
                Id = item.Id
            };
        }
        else
            throw new DalDoesNotExistException("Call with this ID does not exist.");
    }
}