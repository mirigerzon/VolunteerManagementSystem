namespace Dal;
using DalApi;
sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IConfig Config { get; } = new ConfigImplementation();    
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.reset();
    }
}