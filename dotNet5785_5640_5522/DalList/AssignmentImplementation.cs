namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using static DO.Enums;

public class AssignmentImplementation : IAssignment
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
        DataSource.AssignmentsList.Add(newAssignment);
    }
    public void Delete(int id)
    {
        var AssignmentToRemove = DataSource.AssignmentsList.Find(x => x.Id == id);
        if (AssignmentToRemove != null)
            DataSource.AssignmentsList.Remove(AssignmentToRemove);
        else
            throw new Exception("Assignment with this ID does not exist.");
    }
    public void DeleteAll()
    {
        DataSource.AssignmentsList.Clear();
        Console.WriteLine("\n There are no assignments in Assignment list");
    }
    public Assignment? Read(int id)
    {
        return DataSource.AssignmentsList.Find(x => x.Id == id);
    }
    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.AssignmentsList);
    }
    public void Update(Assignment item)
    {
        var existingAssignment = DataSource.AssignmentsList.Find(x => x.Id == item.Id);
        if (existingAssignment != null)
        {
            Delete(existingAssignment.Id);
            DataSource.AssignmentsList.Add(item);
        }
        else
            throw new Exception("Assignment with this ID does not exist.");
    }
}