namespace Dal;
using DalApi;
using DO;
using System.Linq;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
            throw new DalAlreadyExistsException("\n Volunteer with this ID already exists");
        DataSource.Volunteers.Add(item);
    }
    public void Delete(int id)
    {
        var VolunteerToRemove = DataSource.Volunteers.Find(x => x.Id == id);
        if (VolunteerToRemove != null)
            DataSource.Volunteers.Remove(VolunteerToRemove);
        else
            throw new DalDoesNotExistException("\n Volunteer with this ID does not exist.");
    }
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
        Console.WriteLine("\n There are no volunteers in Volunteer list");
    }
    public Volunteer? Read(int id)
    {
    Volunteer? volunteer = DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
        if (volunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID {id} does not exist.");
        return volunteer;
    }
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        Volunteer? volunteer = DataSource.Volunteers.FirstOrDefault(filter);
        if (volunteer == null)
            throw new DalDoesNotExistException("No volunteer matches the provided filter.");
        return volunteer;
    }
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        IEnumerable<Volunteer> results = filter != null
            ? from item in DataSource.Volunteers where filter(item) select item
            : DataSource.Volunteers;
        if (!results.Any())
            Console.WriteLine("No volunteers found matching the filter.");
        return results;
    }
    public void Update(Volunteer item)
    {
        var existingVolunteer = DataSource.Volunteers.Find(x => x.Id == item.Id);
        if (existingVolunteer != null)
        {
            Delete(existingVolunteer.Id);
            DataSource.Volunteers.Add(item);
        }
        else
            throw new DalDoesNotExistException("Volunteer with this ID does not exist.");
    }
}