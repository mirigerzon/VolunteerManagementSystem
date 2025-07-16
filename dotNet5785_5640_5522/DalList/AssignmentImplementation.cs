namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using static DO.Enums;

internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        Assignment newAssignment = new Assignment(
            Config.NextAssignmentId,
            item.VolunteerId,
            item.CallId,
            item.ArrivalTime,
            item.EndTime,
            item.EndStatus
        );
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(x => x.Id == item.Id);
        if (assignment != null)
            throw new DalDoesNotExistException("Volunteer with this ID does exist.");
        DataSource.Assignments.Add(newAssignment);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        var AssignmentToRemove = DataSource.Assignments.Find(x => x.Id == id);
        if (AssignmentToRemove != null)
            DataSource.Assignments.Remove(AssignmentToRemove);
        else
            throw new DalDoesNotExistException("Assignment with this ID does not exist.");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
        Console.WriteLine("\n There are no assignments in Assignment list");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int? id)
    {
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(item => item.Id == id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Assignment with ID {id} does not exist.");
        return assignment;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(filter);
        return assignment;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        IEnumerable<Assignment> results = filter != null
            ? from item in DataSource.Assignments where filter(item) select item
            : DataSource.Assignments;
        if (!results.Any())
            Console.WriteLine("No assignments found matching the filter.");
        return results;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        var existingAssignmentIndex = DataSource.Assignments.FindIndex(x => x.Id == item.Id);
        if (existingAssignmentIndex != -1)
        {
            DataSource.Assignments[existingAssignmentIndex] = new Assignment(
                item.Id,
                item.VolunteerId,
                item.CallId,
                item.ArrivalTime,
                item.EndTime
            )
            {
                Id = item.Id
            };
        }
        else
            throw new DalDoesNotExistException("Assignment with this ID does not exist.");
    }
}