namespace DalTest;
using DalApi;
using DO;

internal static class Initialization
{
    private static IVolunteer? s_dalVolunteer;
    private static ICall? s_dalCall;
    private static IAssignment? s_dalAssignment;
    private static IConfig? s_dalConfig;
    private static readonly Random s_rand = new();

    private static string[] FirstNames = {
    "Michael", "Sarah", "David", "Emma", "Daniel",
    "Sophia", "James", "Olivia", "John", "Mia"
};
    private static string[] LastNames = {
    "Smith", "Johnson", "Williams", "Brown", "Jones",
    "Garcia", "Miller", "Davis", "Martinez", "Hernandez"
};
    private static string[] PhoneNumbers = {
    "0501234567", "0509876543", "0543216789", "0526547890", "0534567890",
    "0501112233", "0512345678", "0545678901", "0527890123", "0506789012"
};
    private static string[] Emails = {
    "michael.smith@example.com", "sarah.johnson@example.com", "david.williams@example.com",
    "emma.brown@example.com", "daniel.jones@example.com", "sophia.garcia@example.com",
    "james.miller@example.com", "olivia.davis@example.com", "john.martinez@example.com",
    "mia.hernandez@example.com"
};
    private static string[] Passwords = {
    "Pass123!", "Secure456!", "MyPass789!", "HelloWorld1!", "TopSecret2!",
    "QuickFox3!", "LazyDog4!", "StrongPass5!", "UniqueCode6!", "SafePassword7!"
};
    private static string[] VolunteerAddresses = {
    "123 Main St, Tel Aviv", "456 Elm St, Haifa", "789 Oak St, Jerusalem",
    "321 Pine St, Be'er Sheva", "654 Maple St, Eilat", "987 Cedar St, Nazareth",
    "234 Birch St, Netanya", "567 Willow St, Ashdod", "890 Ash St, Holon",
    "112 Cherry St, Petah Tikva"
};
    private static string[] Descriptions = {
    "Medical emergency reported",
    "Fire breakout in a building",
    "Car accident on the highway",
    "Lost child in the park",
    "Suspicious activity reported",
    "Water pipe burst at home",
    "Power outage in the neighborhood",
    "Animal stuck in a tree",
    "Flooding due to heavy rains",
    "Gas leak reported in an apartment"
};
    private static string[] CallerAddresses = {
    "123 Elm St, Tel Aviv",
    "456 Oak St, Haifa",
    "789 Pine St, Jerusalem",
    "321 Maple St, Eilat",
    "654 Birch St, Be'er Sheva",
    "987 Cedar St, Netanya",
    "234 Willow St, Ashdod",
    "567 Cherry St, Holon",
    "890 Ash St, Nazareth",
    "112 Walnut St, Petah Tikva"
};

    private static void CreateVolunteer()
    {
        // Create and add a manager if not already exists
        Volunteer manager = new Volunteer
        {
            Id = s_rand.Next(200000000, 400000000),
            FirstName = "Admin",
            LastName = "Manager",
            PhoneNumber = "0521112222",
            Email = "admin.manager@example.com",
            Password = "AdminPass123!",
            Address = "Head Office, Jerusalem",
            Latitude = s_rand.NextDouble() * (32.2 - 29.5) + 29.5, // Random latitude
            Longitude = s_rand.NextDouble() * (35.7 - 34.3) + 34.3, // Random longitude
            Role = RoleEnum.Mentor,
            IsActive = true,
            TypeOfDistance = (TypeOfDistanceEnum)s_rand.Next(0, 3),
        };
        // Check if the manager ID already exists in the system, and add it to the data layer
        Volunteer? checkManager = s_dalVolunteer?.Read(manager.Id);
        if (checkManager == null)
            s_dalVolunteer?.Create(manager);
        // Create and add 10 volunteers
        for (int i = 0; i < 10; i++)
        {
            Volunteer volunteer = new Volunteer
            {
                Id = s_rand.Next(200000000, 400000000),
                FirstName = FirstNames[i],
                LastName = LastNames[i],
                PhoneNumber = PhoneNumbers[i],
                Email = Emails[i],
                Password = Passwords[i],
                Address = VolunteerAddresses[i],
                Latitude = s_rand.NextDouble() * (32.2 - 29.5) + 29.5, // Random latitude
                Longitude = s_rand.NextDouble() * (35.7 - 34.3) + 34.3, // Random longitude
                Role = RoleEnum.Volunteer,
                IsActive = true,
                TypeOfDistance = (TypeOfDistanceEnum)s_rand.Next(0, 3),
            };
            // Check if the volunteer ID already exists in the system, and add it to the data layer
            Volunteer? checkVolunteer = s_dalVolunteer?.Read(volunteer.Id);
            if (checkVolunteer == null)
                s_dalVolunteer.Create(volunteer);
        }
    }
    private static void CreateCall()
    {
        // Create and add 10 calls
        for (int i = 0; i < 10; i++)
        {
            // Define a starting date two years prior to the current date
            // Calculate the range of days between the start date and the current time
            // Generate a random start time within the range
            DateTime startDate = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);
            int range = (s_dalConfig.Clock - startDate).Days;
            DateTime randomStartTime = startDate.AddDays(s_rand.Next(range));
            Call call = new Call
            {
                Status = (CallStatusEnum)s_rand.Next(0, 3),
                Description = Descriptions[i],
                CallerAddress = CallerAddresses[i],
                Latitude = s_rand.NextDouble() * (32.2 - 29.5) + 29.5, // Random latitude
                Longitude = s_rand.NextDouble() * (35.7 - 34.3) + 34.3, // Random longitude
                StartTime = randomStartTime,
                MaxEndTime = null,
            };
            // Check if the call ID already exists in the system
            Call? checkCall = s_dalCall?.Read(call.Id);
            if (checkCall == null)
                // Add the call if it doesn't already exist
                s_dalCall?.Create(call);
        }
    }
    private static void createAssignment()
    {
        // Ensure that volunteers, calls, and assignment dependencies are initialized
        if (s_dalVolunteer == null || s_dalCall == null || s_dalAssignment == null)
            throw new Exception("Dependencies not initialized.");
        // Fetch all existing volunteers and calls
        var allVolunteers = s_dalVolunteer?.ReadAll().ToList();
        var allCalls = s_dalCall?.ReadAll().ToList();
        // Ensure that volunteers and calls are available before proceeding
        if (allVolunteers == null || allCalls == null || !allVolunteers.Any() || !allCalls.Any())
            throw new Exception("Volunteers or Calls data is missing.");
        // Define a starting date two years prior to the current date
        // Calculate the range of days between the start date and the current time
        // Generate a random start time within the range
        DateTime startDate = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);
        int range = (s_dalConfig.Clock - startDate).Days;
        DateTime randomStartTime = startDate.AddDays(s_rand.Next(range));
        // Create 10 random assignments
        for (int i = 0; i < 10; i++)
        {
            var volunteer = allVolunteers[s_rand.Next(allVolunteers.Count)];
            var call = allCalls[s_rand.Next(allCalls.Count)];
            Assignment assignment = new Assignment
            {
                VolunteerId = volunteer.Id,
                CallId = call.Id,
                ArrivalTime = randomStartTime,
                EndTime = null,
                Status = (TerminationTypeEnum)s_rand.Next(0, 3)
            };
            // Check if the assignment ID already exists in the system, and add it to the data layer
            Assignment? checkAssignment = s_dalAssignment?.Read(assignment.Id);
            if (checkAssignment == null)
                s_dalAssignment?.Create(assignment);
        }
    }

}
