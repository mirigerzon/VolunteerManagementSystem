namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        if (Read(item.Id) != null)
            throw new Exception("Assignment with this ID already exists");
        DataSource.AssignmentsList.Add(item);
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