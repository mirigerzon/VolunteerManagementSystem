namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{
    // Implements the creation of a new assignment and saves it to XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);
    }
    // Deletes an assignment by its ID from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);
    }
    // Deletes all assignments from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_Assignments_xml);
    }
    // Reads and retrieves a specific assignment by its ID from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int? id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        return Assignments.FirstOrDefault(it => it.Id == id);
    }
    // Reads and retrieves a specific assignment that matches the given filter condition.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        return Assignments.FirstOrDefault(filter);
    }
    // Reads and retrieves all assignments or those matching the filter condition from the XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        if (filter == null)
            return assignments;
        return assignments.Where(filter);
    }
    // Updates an existing assignment by replacing it with the new version in XML storage.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_Assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignments with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_Assignments_xml);
    }
}