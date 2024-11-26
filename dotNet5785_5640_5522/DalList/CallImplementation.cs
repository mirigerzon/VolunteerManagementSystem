namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        if (Read(item.Id) != null)
            throw new Exception("call with this ID already exists");
        DataSource.CallsList.Add(item);
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
    }
    public Call? Read(int id)
    {
        return DataSource.CallsList.Find(x => x.Id == id);
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