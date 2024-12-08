namespace Dal;
using DO;
internal static class DataSource
{
    internal static List<Volunteer> Volunteers{ get; } = new();
    internal static List<Call> Calls { get; } = new();
    internal static List<Assignment> Assignments { get; } = new();
}