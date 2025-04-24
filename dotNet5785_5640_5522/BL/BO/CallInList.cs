using System;

namespace BO
{
    public class CallInList
    {
        public int Id { get; set; } 
        public int CallId { get; set; } 
        public CallType CallType { get; set; }
        public DateTime? StartTime { get; set; } 
        public TimeSpan? TimeLeft { get; set; } 
        public string? LastVolunteerName { get; set; }
        public TimeSpan? TreatmentDuration { get; set; } 
        public CallStatus Status { get; set; } 
        public int TotalAssignments { get; set; }
    }
}
