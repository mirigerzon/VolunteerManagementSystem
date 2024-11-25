namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        Assignment newAssignment = new Assignment
        {
            Id = Config.NextAssignmentId,
            CallId = item.CallId,
            VolunteerId = item.VolunteerId,
            ArrivalTime = item.ArrivalTime,
            EndTime = item.EndTime,
            Status = item.Status
        };
        DataSource.Assignment.Add(newAssignment);
    }

    public void Delete(int id)
    {
        var AssignmentToRemove = DataSource.Assignment.Find(x => x.Id == id);
        if (AssignmentToRemove != null)
            DataSource.Assignment.Remove(AssignmentToRemove);
        else
            throw new Exception("Assignment with this ID does not exist.");
    }

    public void DeleteAll()
    {
        DataSource.Assignment.Clear();
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignment.Find(x => x.Id == id);
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignment);
    }

    public void Update(Assignment item)
    {
        var existingAssignment = DataSource.Assignment.Find(x => x.Id == item.Id);
        if (existingAssignment != null)
        {
            Delete(existingAssignment.Id);
            DataSource.Assignment.Add(item);
        }
        else
            throw new Exception("Assignment with this ID does not exist.");
    }
}