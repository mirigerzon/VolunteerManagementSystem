namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    // Implements the creation of a new call and saves it to XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (calls.Exists(it => it.Id == item.Id))
            throw new DalAlreadyExistsException($"Call with ID {item.Id} already exists.");

        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_Calls_xml);
    }
    // Deletes a call by its ID from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID {id} not found.");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_Calls_xml);
    }
    // Deletes all calls from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_Calls_xml);
    }
    // Reads and retrieves a specific call by its ID from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int? id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        return calls.FirstOrDefault(it => it.Id == id);
    }
    // Reads and retrieves a specific call that matches the given filter condition.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        return calls.FirstOrDefault(filter);
    }
    // Reads and retrieves all calls or those matching the filter condition from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (filter == null)
            return calls;
        return calls.Where(filter);
    }
    // Updates an existing call by replacing it with the new version in XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_Calls_xml);
        if (calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID {item.Id} not found.");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_Calls_xml);
    }
}