using System;
using System.Net;
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
    (
        int Id,
        CallStatusEnum? Status,
        string Description,
        string CallerAddress,
        double Latitude,
        double Longitude,
        DateTime? StartTime,
        DateTime? MaxEndTime
    )
{
    public Call() : this(0, null, "", "", 0, 0, null, null) { }
}
public enum CallStatusEnum
{
    New,
    InProgress,
    Resolved
}