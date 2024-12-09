namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using static DO.Enums;

internal class AssignmentImplementation : IAssignment
{ 
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
        DataSource.Assignments.Add(newAssignment);
    }
    public void Delete(int id)
    {
        var AssignmentToRemove = DataSource.Assignments.Find(x => x.Id == id);
        if (AssignmentToRemove != null)
            DataSource.Assignments.Remove(AssignmentToRemove);
        else
            throw new DalDoesNotExistException("Assignment with this ID does not exist.");
    }
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
        Console.WriteLine("\n There are no assignments in Assignment list");
    }
    public Assignment? Read(int id)
    {
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(item => item.Id == id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Assignment with ID {id} does not exist.");
        return assignment;
    }
    public Assignment? ReadToCreate(int id)
    {
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(item => item.Id == id);
        if (assignment == null)
            return null;
        return assignment;
    }
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(filter);
        if (assignment == null)
            throw new DalDoesNotExistException("No assignment matches the provided filter.");
        return assignment;
    }
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        IEnumerable<Assignment> results = filter != null
            ? from item in DataSource.Assignments where filter(item) select item
            : DataSource.Assignments;
        if (!results.Any())
            Console.WriteLine("No assignments found matching the filter.");
        return results;
    }
    public void Update(Assignment item)
    {
        var existingAssignmentIndex = DataSource.Assignments.FindIndex(x => x.Id == item.Id);
        if (existingAssignmentIndex != -1)
        {
            DataSource.Assignments[existingAssignmentIndex] = new Assignment(
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