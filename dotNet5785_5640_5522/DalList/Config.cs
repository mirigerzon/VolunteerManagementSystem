namespace Dal;

internal static class Config
{
    internal const int NextCallId = 1;
    private static int nextAssignmentId = NextCallId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange;

    internal static void Reset()
    {
        nextAssignmentId = NextCallId;
        Clock = DateTime.Now;
    }
}
