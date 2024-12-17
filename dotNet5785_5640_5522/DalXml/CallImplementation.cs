namespace Dal;
using DalApi;
using DalXml;
using DO;
using System;
using System.Collections.Generic;
internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (calls.Exists(it => it.Id == item.Id))
            throw new DalAlreadyExistsException($"Call with ID {item.Id} already exists.");

        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_Calls_xml);
    }
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID {id} not found.");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_Calls_xml);
    }
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_Calls_xml);
    }
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        return calls.FirstOrDefault(it => it.Id == id);
    }
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        return calls.FirstOrDefault(filter);
    }
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (filter == null)
            return calls;
        return calls.Where(filter);
    }
    public Call? ReadToCreate(int id)
    {
        throw new NotImplementedException();
    }
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID {item.Id} not found.");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_Calls_xml);
    }
}