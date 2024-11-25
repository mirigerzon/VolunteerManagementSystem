namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
            throw new Exception("volunteer with this ID already exists");
        DataSource.Volunteer.Add(item);
    }

    public void Delete(int id)
    {
        var VolunteerToRemove = DataSource.Volunteer.Find(x => x.Id == id);
        if (VolunteerToRemove != null)
            DataSource.Volunteer.Remove(VolunteerToRemove);
        else
            throw new Exception("Volunteer with this ID does not exist.");
    }

    public void DeleteAll()
    {
        DataSource.Volunteer.Clear();
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteer.Find(x => x.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteer);
    }

    public void Update(Volunteer item)
    {
        var existingVolunteer = DataSource.Volunteer.Find(x => x.Id == item.Id);
        if (existingVolunteer != null)
        {
            Delete(existingVolunteer.Id);
            DataSource.Volunteer.Add(item);
        }
        else
            throw new Exception("Volunteer with this ID does not exist.");
    }
}