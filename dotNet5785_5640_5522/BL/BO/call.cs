﻿using Helpers;
namespace BO;
public class Call
{
    public int Id { get; set; }
    public CallType Type { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime? MaxEndTime { get; set; }
    public CallStatus Status { get; set; }
    public List<CallAssignInList>? Assignments { get; set; }
    public override string ToString() => this.ToStringProperty();

}