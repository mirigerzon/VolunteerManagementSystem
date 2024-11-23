namespace Dal;
internal static class DataSource
{
    internal static List<DO.Volunteer> Volunteer { get; } = new();
    internal static List<DO.Call> Call { get; } = new();
    internal static List<DO.IAssignment> Assignment { get; } = new();
}
