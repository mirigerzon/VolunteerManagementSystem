namespace Dal;
using DalApi;
using DO;
using System.Linq;
using System.Runtime.CompilerServices;

internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        Volunteer? volunteers = DataSource.Volunteers.FirstOrDefault(x => x.Id == item.Id);
        if (volunteers != null)
            throw new DalDoesNotExistException("Volunteer with this ID does exist. ");
        DataSource.Volunteers.Add(item);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        var VolunteerToRemove = DataSource.Volunteers.Find(x => x.Id == id);
        if (VolunteerToRemove != null)
            DataSource.Volunteers.Remove(VolunteerToRemove);
        else
            throw new DalDoesNotExistException("\n Volunteer with this ID does not exist.");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
        Console.WriteLine("\n There are no volunteers in Volunteer list");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int? id)
    {
        Volunteer? volunteer = DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
        if (volunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID {id} does not exist.");
        return volunteer;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        Volunteer? volunteer = DataSource.Volunteers.FirstOrDefault(filter);
        if (volunteer == null)
            throw new DalDoesNotExistException("No volunteer matches the provided filter.");
        return volunteer;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        IEnumerable<Volunteer> results = filter != null
            ? from item in DataSource.Volunteers where filter(item) select item
            : DataSource.Volunteers;
        if (!results.Any())
            Console.WriteLine("No volunteers found matching the filter.");
        return results;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
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