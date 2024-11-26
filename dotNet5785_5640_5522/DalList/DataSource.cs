namespace Dal;
using DO;
internal static class DataSource
{
    internal static List<Volunteer> VolunteersList{ get; } = new();
    internal static List<Call> CallsList { get; } = new();
    internal static List<Assignment> AssignmentsList { get; } = new();
}