namespace Dal;
using DalApi;
sealed public class DalList : IDal
{
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IConfig Config { get; } = new ConfigImplementation();    

    //Reset all data base
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.reset();
    }
}