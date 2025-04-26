namespace BO
{
    public class OpenCallInList
    {
        public int Id { get; set; } 
        public CallType CallType { get; set; } 
        public string? Description { get; set; } 
        public string Address { get; set; } = string.Empty;
        public DateTime? OpeningTime { get; set; } 
        public DateTime? MaxFinishTime { get; set; } 
        public double DistanceFromVolunteer { get; set; }
    }
}