using Microsoft.VisualBasic;
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
        Enums.CallStatusEnum Status,
        Enums.CallTypeEnum Type,
        string Description,
        string CallerAddress,
        double? Latitude,
        double? Longitude,
        DateTime? StartTime,
        DateTime? MaxEndTime
    )
{
    public Call(int id, Enums.CallTypeEnum type, string description, string callerAddress, double latitude, double longitude)
        : this(id, Enums.CallStatusEnum.New, Enums.CallTypeEnum.Medical, description, callerAddress, latitude, longitude, null, null)
    {
    }
    public Call() : this(0, Enums.CallStatusEnum.New, Enums.CallTypeEnum.Medical, "", "", 0, 0, null, null) { }
}