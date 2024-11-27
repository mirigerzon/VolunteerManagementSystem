namespace Dal;
internal static class Config
{
    internal const int startAssignmentId = 1;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }
    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange;
    internal static void Reset()
    {
        nextAssignmentId = NextAssignmentId;
        nextCallId = NextCallId;
        Clock = DateTime.Now;
    }
}