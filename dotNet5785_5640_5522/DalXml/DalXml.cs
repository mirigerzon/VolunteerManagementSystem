namespace Dal;
using DalApi;
using System.Diagnostics;

sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }
    // Provides access to call-related operations.
    public ICall Call { get; } = new CallImplementation();
    // Provides access to volunteer-related operations.
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    // Provides access to assignment-related operations.
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    // Provides access to configuration-related operations.
    public IConfig Config { get; } = new ConfigImplementation();
    // Resets the entire database by clearing all entities and resetting configuration values.
    public void ResetDB()
    {
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Config.reset();
    }
}