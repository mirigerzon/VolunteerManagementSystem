using DO;
using Helpers;
namespace BO;
public class Call
{
    public int Id { get; set; }
    public CallType Type { get; set; }
    public string? Description { get; set; }
    public string? CallerAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? MaxEndTime { get; set; }
    public CallStatus Status { get; set; }
    public List<CallAssignInList>? Assignments { get; set; }
    public override string ToString()
    {
        return $"\n ------------------------------- \n " +
            $"Id: {Id} \n " +
            $"Description: {Description}\n " +
            $"StartTime: {StartTime}\n " +
            $"Status: {Status}\n " +
            $"Assignment - \n{string.Join("\n", Assignments ?? new())}\n " +
            $" ------------------------------- \n";
    }
}