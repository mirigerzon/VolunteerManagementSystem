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
    "Sophia", "James", "Olivia", "John", "Mia",
    "Ethan", "Isabella", "Noah", "Liam", "Ava"
};
    private static string[] LastNames = {
    "Smith", "Johnson", "Williams", "Brown", "Jones",
    "Garcia", "Miller", "Davis", "Martinez", "Hernandez",
    "Clark", "Lopez", "Taylor", "Lee", "Anderson"
};
    private static string[] PhoneNumbers = {
    "0501234567", "0509876543", "0543216789", "0526547890", "0534567890",
    "0501112233", "0512345678", "0545678901", "0527890123", "0506789012",
    "0523334444", "0532221111", "0545556666", "0507778888", "0519990000"
};
    private static string[] Emails = {
    "michael.smith@example.com", "sarah.johnson@example.com", "david.williams@example.com",
    "emma.brown@example.com", "daniel.jones@example.com", "sophia.garcia@example.com",
    "james.miller@example.com", "olivia.davis@example.com", "john.martinez@example.com",
    "mia.hernandez@example.com", "ethan.clark@example.com", "isabella.lopez@example.com",
    "noah.taylor@example.com", "liam.lee@example.com", "ava.anderson@example.com"
};
    private static string[] Passwords = {
    "Pass123!", "Secure456!", "MyPass789!", "HelloWorld1!", "TopSecret2!",
    "QuickFox3!", "LazyDog4!", "StrongPass5!", "UniqueCode6!", "SafePassword7!",
    "BetterSafe8!", "UltraSecure9!", "StrongKey10!", "SafeHouse11!", "SecureDoor12!"
};
    private static double[] VolunteerLongitudes = {
    34.7818, 34.7821, 34.7823, 34.7825, 34.7819,
    34.7816, 34.7820, 34.7822, 34.7817, 34.7824,
    34.7815, 34.7818, 34.7821, 34.7816, 34.7819
};
    private static double[] VolunteerLatitudes = {
    32.0853, 32.0855, 32.0854, 32.0852, 32.0856,
    32.0853, 32.0855, 32.0854, 32.0852, 32.0856,
    32.0853, 32.0855, 32.0854, 32.0852, 32.0856
};
    private static string[] VolunteerAddresses = {
    "123 Rothschild Blvd, Tel Aviv",
    "456 Allenby St, Tel Aviv",
    "789 Ben Yehuda St, Tel Aviv",
    "321 Dizengoff St, Tel Aviv",
    "654 Florentin St, Tel Aviv",
    "987 Arlozorov St, Tel Aviv",
    "234 HaYarkon St, Tel Aviv",
    "567 Jabotinsky St, Tel Aviv",
    "890 Basel St, Tel Aviv",
    "112 Herzl St, Tel Aviv",
    "210 Bograshov St, Tel Aviv",
    "341 Lilienblum St, Tel Aviv",
    "672 Ibn Gabirol St, Tel Aviv",
    "893 Sheinkin St, Tel Aviv",
    "114 Yehuda Halevi St, Tel Aviv"
};

    private static string[] Descriptions = {
    "Flat tire in need of assistance",
    "Car battery drained - requires jump start",
    "Locked car with keys inside",
    "Car stuck in mud and needs towing",
    "Baby stroller stuck in an elevator",
    "Home door jammed - needs help to open",
    "Electric bike with a dead battery",
    "Elderly person needs help fetching medications",
    "Family stranded on the road without fuel",
    "Overheated car needs radiator water",
    "Assistance with changing a heavy vehicle's tire",
    "Box locked without a key available",
    "Help delivering food to the needy",
    "Lost personal item in a public area",
    "Shopping cart stuck in an elevator",
    "Heavy equipment fell on the road",
    "Child stuck with their hand in a railing",
    "Rescue of a baby locked inside a car",
    "Unloading vehicle carrying goods for charity",
    "Help with household tasks for the elderly",
    "Car door handle broken and needs fixing",
    "Motorcycle stranded due to mechanical failure",
    "Help securing loose loads on a truck",
    "Bike chain stuck and needs repair",
    "Towing assistance for a stranded vehicle",
    "Providing blankets and water in cold weather",
    "Car slid off the road in icy conditions",
    "Elderly person locked out of their home",
    "Help carrying groceries to an upper floor",
    "Assisting a family with moving furniture",
    "Rescue of a vehicle stranded in flooding",
    "Temporary shelter setup for displaced families",
    "Help removing a stuck shopping cart wheel",
    "Baby’s crib jammed and needs fixing",
    "Repair of a malfunctioning wheelchair ramp",
    "Opening a safe box with a lost key",
    "Repairing a punctured bicycle tire",
    "Rescue of a pet locked inside a vehicle",
    "Providing first aid to an injured cyclist",
    "Removing debris blocking a small roadway",
    "Helping push a car stuck in sand",
    "Clearing branches obstructing a pathway",
    "Providing fuel to a stranded motorist",
    "Carrying heavy packages for an elderly individual",
    "Organizing and delivering donated clothing",
    "Unblocking a door locked from the inside",
    "Clearing water from a flooded basement",
    "Child stuck in a swing at the park",
    "Fixing a broken stair railing at home",
    "Providing support to a stranded hitchhiker"
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
    "112 Walnut St, Petah Tikva",
    "98 Spruce St, Rishon LeZion",
    "77 Palm St, Kfar Saba",
    "55 Redwood St, Bat Yam",
    "22 Cypress St, Herzliya",
    "10 Acacia St, Modi'in",
    "88 Magnolia St, Nahariya",
    "66 Poplar St, Ra'anana",
    "44 Chestnut St, Hadera",
    "33 Alder St, Acre",
    "111 Olive St, Safed",
    "12 Plum St, Ramat Gan",
    "34 Sycamore St, Beit Shemesh",
    "67 Bamboo St, Lod",
    "99 Hazel St, Tiberias",
    "25 Maple Leaf St, Ariel",
    "53 Coral St, Giv'atayim",
    "72 Sage St, Karmiel",
    "14 Lilac St, Ma'ale Adumim",
    "39 Myrtle St, Yavne",
    "81 Juniper St, Sderot",
    "18 Aspen St, Dimona",
    "76 Hawthorn St, Migdal HaEmek",
    "45 Eucalyptus St, Kiryat Gat",
    "29 Pinecone St, Afula",
    "31 Ivy St, Kiryat Malakhi",
    "20 Fir St, Rosh HaAyin",
    "59 Lavender St, Ashkelon",
    "93 Marigold St, Qiryat Shemona",
    "17 Sagebrush St, Yehud",
    "36 Daffodil St, Or Akiva",
    "84 Camellia St, Nesher",
    "50 Fern St, Betar Illit",
    "41 Amaranth St, Bnei Brak",
    "26 Daisy St, Kiryat Arba",
    "15 Carnation St, Efrat",
    "63 Bluebell St, Zikhron Ya'akov",
    "19 Gardenia St, Gan Yavne",
    "78 Gladiolus St, Ofakim",
    "11 Oleander St, Mevaseret Zion",
    "95 Buttercup St, Even Yehuda"
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
            Address = "Head Office, Tel Aviv-Jafo",
            Latitude = "32.085299",
            Longitude = "34.781769",
            Role = RoleEnum.Mentor,
            IsActive = true,
            MaxOfDistance = s_rand.NextDouble() * (50 - 5) + 5,
            TypeOfDistance = (TypeOfDistanceEnum)s_rand.Next(0, 3),
        };
        // Check if the manager ID already exists in the system, and add it to the data layer
        Volunteer? checkManager = s_dalVolunteer?.Read(manager.Id);
        if (checkManager == null)
            s_dalVolunteer?.Create(manager);
        // Create and add 10 volunteers
        for (int i = 0; i < 15; i++)
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
                Latitude = VolunteerLatitudes[i],
                Longitude = VolunteerLongitudes[i],
                Role = RoleEnum.Volunteer,
                IsActive = true,
                MaxOfDistance = s_rand.NextDouble() * (50 - 5) + 5,
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
        int expiredCalls = 0;
        int totalCalls = 0;

        for (int i = 0; i < 50; i++)
        {
            DateTime startDate = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);
            int range = (s_dalConfig.Clock - startDate).Days;
            DateTime randomStartTime = startDate.AddDays(s_rand.Next(range));
            DateTime? randomMaxEndTime = (expiredCalls < 5 && s_rand.Next(0, 5) == 0)
                ? null : (s_rand.Next(0, 2) == 0 ? null : randomStartTime.AddHours(s_rand.Next(1, 48)));

            Call call = new Call
            {
                Status = (s_rand.Next(0, 4) == 0)
                    ? CallStatusEnum.New : (CallStatusEnum)s_rand.Next(0, 3),
                Description = Descriptions[i],
                CallerAddress = CallerAddresses[i],
                Latitude = s_rand.NextDouble() * (32.2 - 29.5) + 29.5,
                Longitude = s_rand.NextDouble() * (35.7 - 34.3) + 34.3,
                StartTime = randomStartTime,
                MaxEndTime = randomMaxEndTime,
            };

            if (randomMaxEndTime == null && expiredCalls < 5)
                expiredCalls++;

            Call? checkCall = s_dalCall?.Read(call.Id);
            if (checkCall == null)
                s_dalCall?.Create(call);
            totalCalls++;
        }
    }

    private static void CreateAssignment()
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
        // Create 35 random assignments (at least 15 unassigned) 
        for (int i = 0; i < 35; i++)
        {
            var volunteer = allVolunteers[s_rand.Next(allVolunteers.Count)];
            var call = allCalls[i];
            Assignment assignment = new Assignment
            {
                VolunteerId = volunteer.Id,
                CallId = call.Id,
                ArrivalTime = (call.MaxEndTime != null && s_rand.Next(0, 10) < 9) ?
                    call.StartTime.AddMinutes(s_rand.Next(1, (int)((call.MaxEndTime - call.StartTime)?.TotalMinutes ?? 60))) : null,
                EndTime = s_rand.Next(0, 2) == ? null
                : randomStartTime.AddMinutes(s_rand.Next(1, 181)),
                EndStatus = (call.MaxEndTime != null && call.StartTime <= call.MaxEndTime)
                    ? TerminationTypeEnum.Treated
                    : (call.MaxEndTime == null)
                    ? (TerminationTypeEnum)s_rand.Next(1, 3)
                    : TerminationTypeEnum.Expired;
            };

            // Check if the assignment ID already exists in the system, and add it to the data layer
            Assignment? checkAssignment = s_dalAssignment?.Read(assignment.Id);
            if (checkAssignment == null)
                s_dalAssignment?.Create(assignment);
        }
    }

    public static void Do(IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");
        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalVolunteer.DeleteAll();
        s_dalAssignment.DeleteAll();
        s_dalCall.DeleteAll();
        Console.WriteLine("Initializing Students list ...");
        CreateVolunteer();
        CreateCall();
        CreateAssignment();
    }
}