using System;

namespace BO
{
    public class CallInList
    {
        public int Id { get; set; } // מזהה ההקצאה (לא מוצג אך קיים)

        public int CallId { get; set; } // מזהה הקריאה הרץ

        public CallType CallType { get; set; } // ENUM מסוג הקריאה

        public DateTime OpeningTime { get; set; } // זמן פתיחה

        public TimeSpan? TimeLeft { get; set; } // סך הזמן שנותר לסיום הקריאה

        public string? LastVolunteerName { get; set; } // שם מתנדב אחרון

        public TimeSpan? TreatmentDuration { get; set; } // סך זמן הטיפול

        public CallStatus Status { get; set; } // ENUM סטטוס הקריאה

        public int TotalAssignments { get; set; } // סך ההקצאות
    }
}
