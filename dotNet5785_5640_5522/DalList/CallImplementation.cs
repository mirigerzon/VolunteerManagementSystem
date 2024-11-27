namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        Call newCall = new Call(
            Config.NextCallId,
            item.Status,
            item.Description,
            item.CallerAddress,
            item.Latitude,
            item.Longitude,
            item.StartTime,
            item.MaxEndTime
        );
        DataSource.CallsList.Add(newCall);
    }
    public void Delete(int id)
    {
        var callToRemove = DataSource.CallsList.Find(x => x.Id == id);
        if (callToRemove != null)
            DataSource.CallsList.Remove(callToRemove);
        else
            throw new Exception("Call with this ID does not exist.");
    }
    public void DeleteAll()
    {
        DataSource.CallsList.Clear();
        Console.WriteLine("\n There are no calls in Call list");
    }
    public Call? Read(int id)
    {
        if (DataSource.CallsList != null)
            return DataSource.CallsList.Find(x => x.Id == id);
        else throw new Exception($"\n Call with id {id} is undefined");
    }
    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.CallsList);
    }
    public void Update(Call item)
    {
        var existingCall = DataSource.CallsList.Find(x => x.Id == item.Id);
        if (existingCall != null)
        {
            Delete(existingCall.Id);
            DataSource.CallsList.Add(item);
        }
        else
            throw new Exception("Call with this ID does not exist.");
    }
}