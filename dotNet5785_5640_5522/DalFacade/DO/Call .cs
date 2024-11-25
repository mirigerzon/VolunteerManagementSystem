using System.Net.NetworkInformation;

namespace DO;

/// <summary>
/// Call Entity represents a call with all its properties.
/// </summary>
/// <param name="Id">Unique ID of the call.</param>
/// <param name="Status">Current status of the call.</param>
/// <param name="Description">Description of the call (default null).</param>
/// <param name="CallerAddress">Address of the caller.</param>
/// <param name="Latitude">Latitude of the caller's location (default null).</param>
/// <param name="Longitude">Longitude of the caller's location (default null).</param>
/// <param name="StartTime">Start time of the call.</param>
/// <param name="MaxEndTime">Maximum end time allowed for the call (default null).</param>
public record Call
{
    public int Id { get; set; }
    public CallStatusEnum Status { get; set; }
    public string Description { get; set; } = null;
    public string CallerAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? MaxEndTime { get; set; } = null;
    public Call() { }
}
public enum CallStatusEnum
{
    New,
    InProgress,
    Resolved
}