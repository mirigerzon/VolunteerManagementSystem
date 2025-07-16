namespace DalTest;
using DalApi;
using DO;
using static DO.Enums;
public static class Initialization
{
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();
    private static string[] FullNames = {
        "John Smith", "Sarah Johnson", "Michael Williams", "Emma Brown", "Daniel Jones",
        "Sophia Garcia", "James Miller", "Olivia Davis", "David Martinez", "Mia Hernandez",
        "Ethan Clark", "Isabella Lopez", "Noah Taylor", "Liam Lee", "Ava Anderson"
    };
    private static string[] PhoneNumbers = {
        "0583286530", "0583286530", "0583286530", "0583286530", "0583286530",
        "0583286530", "0583286530", "0583286530", "0583286530", "0583286530",
        "0583286530", "0583286530", "0583286530", "0583286530", "0583286530"
    };
    private static string[] Emails = {
        "john.smith@example.com", "sarah.johnson@example.com", "michael.williams@example.com",
        "emma.brown@example.com", "daniel.jones@example.com", "sophia.garcia@example.com",
        "james.miller@example.com", "olivia.davis@example.com", "david.martinez@example.com",
        "mia.hernandez@example.com", "ethan.clark@example.com", "isabella.lopez@example.com",
        "noah.taylor@example.com", "liam.lee@example.com", "ava.anderson@example.com"
    };
    private static string[] Passwords = {
        "Pass123!", "Secure456!", "MyPass789!", "HelloWorld1!", "TopSecret2!",
        "QuickFox3!", "LazyDog4!", "StrongPass5!", "UniqueCode6!", "SafePassword7!",
        "BetterSafe8!", "UltraSecure9!", "StrongKey10!", "SafeHouse11!", "SecureDoor12!"
    };

    // Real addresses with corresponding coordinates
    private static string[] VolunteerAddresses = {
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
        "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA",
    };

    // Coordinates for volunteer addresses (lat, lon)
    private static double[] VolunteerLatitudes = {
        37.4220, 37.4843, 47.6205, 37.3346, 47.6398,
        42.3626, 40.7590, 37.7749, 37.3688, 37.4419,
        37.6213, 30.2672, 37.5407, 34.0522, 41.8781
    };
    private static double[] VolunteerLongitudes = {
        -122.0841, -122.1477, -122.3493, -122.0090, -122.1283,
        -71.0843, -73.9845, -122.4194, -122.0363, -122.1430,
        -122.3805, -97.7431, -122.2751, -118.2437, -87.6298
    };

    private static string[] Descriptions = {
        "Flat tire in parking lot needs assistance",
        "Car battery dead - requires jump start",
        "Locked keys inside vehicle",
        "Car stuck in parking garage",
        "Elderly person needs help with groceries",
        "Wheelchair accessibility issue at building entrance",
        "Electric scooter battery died",
        "Person needs help carrying heavy items upstairs",
        "Family stranded - car won't start",
        "Need help changing tire on busy street",
        "Medical emergency - person needs transport",
        "Lost wallet in public area - help searching",
        "Food delivery for homeless shelter",
        "Pet locked in car - urgent rescue needed",
        "Moving assistance for elderly neighbor",
        "Car overheated on highway",
        "Child separated from family at mall",
        "Bike chain broken - need repair help",
        "Delivery truck stuck in narrow alley",
        "Person fell and needs first aid",
        "Broken wheelchair needs temporary fix",
        "Lost tourist needs directions and help",
        "Flooded basement needs water removal",
        "Power outage - elderly person needs assistance",
        "Stuck elevator with person inside",
        "Ice storm - person needs help getting home",
        "Flat tire on motorcycle",
        "Person locked out of apartment",
        "Heavy furniture delivery assistance needed",
        "Car accident - need traffic direction help",
        "Diabetic person needs emergency glucose",
        "Phone died - need to contact family urgently",
        "Slippery sidewalk - person fell",
        "Car window broken - need temporary cover",
        "Lost prescription medication",
        "Severe allergic reaction - need EpiPen",
        "Confused elderly person needs help getting home",
        "Broken crutch needs temporary replacement",
        "Person having panic attack needs support",
        "Car won't start in remote location",
        "Person needs help reading medication labels",
        "Wheelchair tire flat - needs replacement",
        "Person with visual impairment needs navigation help",
        "Insulin emergency - diabetic person needs help",
        "Person with hearing aid malfunction needs assistance",
        "Emergency childcare needed - parent hospitalized",
        "Person needs help with translation",
        "Oxygen tank running low - needs refill assistance",
        "Person with mobility issues stuck in snow",
        "Emergency medication delivery needed"
    };

    private static string[] CallerAddresses = {
        "123 Main St, New York, NY 10001, USA",
        "456 Oak Ave, Los Angeles, CA 90210, USA",
        "789 Pine St, Chicago, IL 60601, USA",
        "321 Elm St, Houston, TX 77001, USA",
        "654 Maple Ave, Phoenix, AZ 85001, USA",
        "987 Cedar St, Philadelphia, PA 19101, USA",
        "234 Birch Ave, San Antonio, TX 78201, USA",
        "567 Willow St, San Diego, CA 92101, USA",
        "890 Cherry Ave, Dallas, TX 75201, USA",
        "112 Ash St, San Jose, CA 95101, USA",
        "445 Spruce St, Austin, TX 78701, USA",
        "778 Palm Ave, Jacksonville, FL 32099, USA",
        "556 Redwood St, San Francisco, CA 94102, USA",
        "223 Cypress Ave, Columbus, OH 43085, USA",
        "101 Magnolia St, Charlotte, NC 28202, USA",
        "667 Poplar Ave, Fort Worth, TX 76101, USA",
        "334 Chestnut St, Seattle, WA 98101, USA",
        "889 Alder Ave, Denver, CO 80202, USA",
        "111 Olive St, El Paso, TX 79901, USA",
        "222 Plum Ave, Detroit, MI 48201, USA",
        "555 Sycamore St, Boston, MA 02101, USA",
        "777 Bamboo Ave, Memphis, TN 38101, USA",
        "999 Hazel St, Nashville, TN 37201, USA",
        "333 Coral Ave, Portland, OR 97201, USA",
        "666 Sage St, Oklahoma City, OK 73101, USA",
        "444 Lilac Ave, Las Vegas, NV 89101, USA",
        "888 Myrtle St, Louisville, KY 40202, USA",
        "147 Juniper Ave, Baltimore, MD 21201, USA",
        "258 Aspen St, Milwaukee, WI 53201, USA",
        "369 Hawthorn Ave, Albuquerque, NM 87101, USA",
        "741 Eucalyptus St, Tucson, AZ 85701, USA",
        "852 Pinecone Ave, Fresno, CA 93701, USA",
        "963 Ivy St, Sacramento, CA 95814, USA",
        "159 Fir Ave, Long Beach, CA 90801, USA",
        "357 Lavender St, Kansas City, MO 64108, USA",
        "468 Marigold Ave, Mesa, AZ 85201, USA",
        "579 Sagebrush St, Virginia Beach, VA 23451, USA",
        "681 Daffodil Ave, Atlanta, GA 30309, USA",
        "792 Camellia St, Colorado Springs, CO 80903, USA",
        "135 Fern Ave, Omaha, NE 68102, USA",
        "246 Amaranth St, Raleigh, NC 27601, USA",
        "357 Daisy Ave, Miami, FL 33101, USA",
        "468 Carnation St, Oakland, CA 94612, USA",
        "579 Bluebell Ave, Minneapolis, MN 55401, USA",
        "681 Gardenia St, Tulsa, OK 74103, USA",
        "792 Gladiolus Ave, Arlington, TX 76010, USA",
        "135 Oleander St, New Orleans, LA 70112, USA",
        "246 Buttercup Ave, Wichita, KS 67202, USA",
        "357 Hibiscus St, Cleveland, OH 44113, USA",
        "468 Jasmine Ave, Tampa, FL 33602, USA"
    };

    //Creates a manager and multiple volunteers, adding them to the system if they don't already exist.
    private static void CreateVolunteer()
    {
        // Create and add a manager if not already exists
        Volunteer manager = new Volunteer
        (
            328115522,
            "Admin Manager",
            "0583286530",
            "admin.manager@example.com",
            "1111",
            "1 Infinite Loop, Cupertino, CA 95014, USA",
            37.3318,
            -122.0312,
            Enums.RoleEnum.Admin,
            true,
            (s_rand.NextDouble() * (50 - 5) + 5),
            (Enums.TypeOfDistanceEnum)s_rand.Next(0, 3)
        );

        s_dal?.Volunteer.Create(manager);

        // Create and add 15 volunteers
        for (int i = 0; i < 15; i++)
        {
            int volId = s_rand.Next(200000000, 400000000);
            Volunteer volunteer = new Volunteer
            (
                volId,
                FullNames[i],
                PhoneNumbers[i],
                Emails[i],
                Passwords[i],
                VolunteerAddresses[i],
                VolunteerLatitudes[i],
                VolunteerLongitudes[i],
                Enums.RoleEnum.Volunteer,
                true,
                (s_rand.NextDouble() * (50 - 5) + 5),
                (Enums.TypeOfDistanceEnum)s_rand.Next(0, 3)
            );

            s_dal.Volunteer.Create(volunteer);
        }
    }

    //Generates random calls and adds them to the system, ensuring no duplicates exist.
    private static void CreateCall()
    {
        DateTime today = s_dal.Config.Clock;

        for (int i = 0; i < 50; i++)
        {
            // רוב הקריאות יהיו אקטיביות ופתוחות
            DateTime randomStartTime;
            DateTime? randomMaxEndTime;

            if (i < 40) // 40 קריאות אקטיביות מתוך 50
            {
                // קריאות שנפתחו ב-3 הימים האחרונים
                randomStartTime = today.AddDays(-s_rand.Next(0, 4));
                // תוקף יפוג בתוך יום-יומיים
                randomMaxEndTime = today.AddDays(s_rand.Next(2, 4));
            }
            else // 10 קריאות ישנות/סגורות
            {
                // קריאות ישנות מהחודש האחרון
                randomStartTime = today.AddDays(-s_rand.Next(4, 31));
                // תוקף שכבר פג או יפוג בקרוב
                randomMaxEndTime = randomStartTime.AddHours(s_rand.Next(1, 72));
            }

            // Generate random coordinates for US addresses (approximate ranges)
            double callerLat = s_rand.NextDouble() * (49.0 - 25.0) + 25.0; // US latitude range
            double callerLon = s_rand.NextDouble() * (-66.0 - (-125.0)) + (-125.0); // US longitude range

            Call call = new Call(
                s_dal.Config.GetNextCallId(),
                (i < 40) ? CallStatusEnum.Open : CallStatusEnum.Closed,
                (s_rand.Next(0, 5) == 0) ? CallTypeEnum.Medical : (CallTypeEnum)s_rand.Next(0, 4),
                Descriptions[i],
                CallerAddresses[i],
                callerLat,
                callerLon,
                randomStartTime,
                randomMaxEndTime
            );

            s_dal?.Call.Create(call);
        }
    }

    //Initializes the system by resetting data and invoking CreateVolunteer, CreateCall, and CreateAssignment.
    public static void Do()
    {
        s_dal = DalApi.Factory.Get;
        Console.WriteLine("\n Reset Configuration values and List values...");
        s_dal.ResetDB();
        CreateVolunteer();
        Console.WriteLine("\n Initializing volunteer list ...");
        CreateCall();
        Console.WriteLine("\n Initializing call list ...");
        Console.WriteLine("\n Initialization completed successfully!");
    }
}