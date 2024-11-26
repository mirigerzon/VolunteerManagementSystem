namespace Dal;
using DalApi;
using DO;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
            throw new Exception("volunteer with this ID already exists");
        DataSource.VolunteersList.Add(item);
    }
    public void Delete(int id)
    {
        var VolunteerToRemove = DataSource.VolunteersList.Find(x => x.Id == id);
        if (VolunteerToRemove != null)
            DataSource.VolunteersList.Remove(VolunteerToRemove);
        else
            throw new Exception("Volunteer with this ID does not exist.");
    }
    public void DeleteAll()
    {
        DataSource.VolunteersList.Clear();
    }
    public Volunteer? Read(int id)
    {
        if (DataSource.VolunteersList != null)
            return DataSource.VolunteersList.Find(x => x.Id == id);
        else throw new Exception("DataSource.Volunteer is empty");
    }
    public List<Volunteer> ReadAll()
    {
        if (DataSource.VolunteersList != null)
            return new List<Volunteer>(DataSource.VolunteersList);
        else throw new Exception("DataSource.Volunteer is empty");
    }
    public void Update(Volunteer item)
    {
        var existingVolunteer = DataSource.VolunteersList.Find(x => x.Id == item.Id);
        if (existingVolunteer != null)
        {
            Delete(existingVolunteer.Id);
            DataSource.VolunteersList.Add(item);
        }
        else
            throw new Exception("Volunteer with this ID does not exist.");
    }
}