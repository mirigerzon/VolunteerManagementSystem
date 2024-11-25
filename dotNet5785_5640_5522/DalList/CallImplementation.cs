namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        Call newCall = new Call
        {
            Id = Config.NextCallId,
            Status = item.Status,
            Description = item.Description,
            CallerAddress = item.CallerAddress,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            StartTime = item.StartTime,
            MaxEndTime = item.MaxEndTime,
        };
        DataSource.Call.Add(newCall);
    }

    public void Delete(int id)
    {
        var callToRemove = DataSource.Call.Find(x => x.Id == id);
        if (callToRemove != null)
            DataSource.Call.Remove(callToRemove);
        else
            throw new Exception("Call with this ID does not exist.");
    }

    public void DeleteAll()
    {
        DataSource.Call.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Call.Find(x => x.Id == id);
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Call);
    }

    public void Update(Call item)
    {
        var existingCall = DataSource.Call.Find(x => x.Id == item.Id);
        if (existingCall != null)
        {
            Delete(existingCall.Id);
            DataSource.Call.Add(item);
        }
        else
            throw new Exception("Call with this ID does not exist.");
    }
}