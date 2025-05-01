using System;
using System.Data;
using System.Net;
using System.Numerics;
using BlApi;
using BO;
using DO;

namespace BlTest
{
    class Program
    {
        static readonly IBl s_bl = Factory.Get();
        // Entry point of the program with main menu navigation.
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Volunteer operations");
                Console.WriteLine("2. Call operations");
                Console.WriteLine("3. Admin operations");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        VolunteerMenu();
                        break;
                    case "2":
                        CallMenu();
                        break;
                    case "3":
                        AdminMenu();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        // Displays the volunteer operations menu and handles user choices.
        private static void VolunteerMenu()
        {
            while (true)
            {
                Console.WriteLine("\nVolunteer Menu:");
                Console.WriteLine("1. Add Volunteer");
                Console.WriteLine("2. Get Volunteer");
                Console.WriteLine("3. Get all Volunteers");
                Console.WriteLine("4. Update Volunteer");
                Console.WriteLine("5. Remove Volunteer");
                Console.WriteLine("6. Back to Main Menu");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddVolunteer();
                            break;
                        case "2":
                            GetVolunteer();
                            break;
                        case "3":
                            foreach (var volunteer in s_bl.Volunteer.ReadAll())
                                Console.WriteLine(volunteer);
                            break;
                        case "4":
                            UpdateVolunteer();
                            break;
                        case "5":
                            RemoveVolunteer();
                            break;
                        case "6":
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
        // Displays the call operations menu and handles user choices.
        private static void CallMenu()
        {
            while (true)
            {
                Console.WriteLine("\nCall Menu:");
                Console.WriteLine("1. Add Call");
                Console.WriteLine("2. Get Call");
                Console.WriteLine("3. Get all Calls");
                Console.WriteLine("4. Update Call");
                Console.WriteLine("5. Remove Call");
                Console.WriteLine("6. Back to Main Menu");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddCall();
                            break;
                        case "2":
                            GetCall();
                            break;
                        case "3":
                            foreach (var call in s_bl.Call.GetCallsList())
                                Console.WriteLine(call);
                            break;
                        case "4":
                            UpdateCall();
                            break;
                        case "5":
                            RemoveCall();
                            break;
                        case "6":
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
        // Displays the admin operations menu and handles user choices.
        private static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\nAdmin Menu:");
                Console.WriteLine("1. Reset Database");
                Console.WriteLine("2. Initialize Database");
                Console.WriteLine("3. Advance System Clock");
                Console.WriteLine("4. Get System Clock");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            s_bl.Admin.ResetDatabase();
                            Console.WriteLine("Database reset.");
                            break;
                        case "2":
                            s_bl.Admin.InitializeDatabase();
                            Console.WriteLine("Database initialized.");
                            break;
                        case "3":
                            Console.WriteLine("Choose time unit to advance: (1) Minute (2) Hour (3) Day");
                            string unitInput = Console.ReadLine();
                            TimeUnit unit = unitInput switch
                            {
                                "1" => TimeUnit.Minute,
                                "2" => TimeUnit.Hour,
                                "3" => TimeUnit.Day,
                                _ => throw new ArgumentException("Invalid time unit")
                            };
                            s_bl.Admin.AdvanceSystemClock(unit);
                            Console.WriteLine("Clock advanced.");
                            break;
                        case "4":
                            Console.WriteLine($"System Clock: {s_bl.Admin.GetSystemClock()}");
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
        // Prompts the user to enter volunteer details and adds the volunteer.
        private static void AddVolunteer()
        {
            Console.Write("Enter Volunteer Id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }

            Console.Write("Enter Full Name: ");
            string fullName = Console.ReadLine();
            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine();
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            Console.Write("Enter Address: ");
            string address = Console.ReadLine();
            Console.Write("Enter maxOfDistance (km): ");
            string maxOfDistance = Console.ReadLine();

            var volunteer = new BO.Volunteer
            {
                Id = id,
                FullName = fullName,
                PhoneNumber = phone,
                Email = email,
                Password = password,
                Address = address,
                IsActive = true,
                Role = UserRole.Volunteer,
                MaxDistance = double.Parse(maxOfDistance),
                TypeOfDistance = TypeOfDistance.Driving
            };

            s_bl.Volunteer.Create(volunteer);
            Console.WriteLine("Volunteer added successfully.");
        }
        // Prompts for a volunteer ID and displays the volunteer's details.
        private static void GetVolunteer()
        {
            Console.Write("Enter Volunteer Id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }

            var volunteer = s_bl.Volunteer.Read(id);
            Console.WriteLine(volunteer.ToString());
        }
        // Prompts for a volunteer ID, updates the volunteer's information.
        private static void UpdateVolunteer()
        {
            Console.Write("Enter Volunteer Id to Update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }
            var volunteer = s_bl.Volunteer.Read(id);
            Console.WriteLine("\n Enter full name");
            string fullName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DalInvalidException("Full name cannot be null or empty");
            Console.WriteLine("\n Enter phoneNumber");
            string phoneNumber = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DalInvalidException("Phone number cannot be null or empty");
            Console.WriteLine("\n Enter email address");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
                throw new DalInvalidException("Email address cannot be null or empty");
            Console.WriteLine("\n Enter password");
            string password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(password))
                throw new DalInvalidException("Password cannot be null or empty");
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new DalInvalidException("Address cannot be null or empty");
            Console.WriteLine("\n Enter maxOfDistance (km)");
            double maxOfDistance;
            if (!double.TryParse(Console.ReadLine(), out maxOfDistance))
                throw new DalInvalidException("\n Invalid input for maxOfDistance");
            Console.WriteLine("Enter role (0 = Manager, 1 = Volunteer):");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out int roleValue) || !Enum.IsDefined(typeof(UserRole), roleValue))
                throw new DalInvalidException("\nInvalid input for role");
            UserRole role = (UserRole)roleValue;

            Console.WriteLine("Enter type (0 = Aerial, 1 = Walking, 2 = Driving):");
            string typeOfDist = Console.ReadLine();
            if (!int.TryParse(typeOfDist, out int typeValue) || !Enum.IsDefined(typeof(TypeOfDistance), typeValue))
                throw new DalInvalidException("\nInvalid input for role");
            TypeOfDistance type = (TypeOfDistance)typeValue;
            var volunteerToUpdate = new BO.Volunteer
            {
                Id = volunteer.Id,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Email = email,
                Password = password,
                Address = address,
                Role = role,
                IsActive = volunteer.IsActive,
                MaxDistance = maxOfDistance,
                TypeOfDistance = type
            };
            s_bl.Volunteer.Update(id, volunteerToUpdate);
            Console.WriteLine("Volunteer updated successfully.");
        }
        // Prompts for a volunteer ID and removes the volunteer.
        private static void RemoveVolunteer()
        {
            Console.Write("Enter Volunteer Id to Remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }
            s_bl.Volunteer.Delete(id);
            Console.WriteLine("Volunteer removed successfully.");
        }
        // Prompts the user to enter call details and adds a new call.
        private static void AddCall()
        {
            Console.WriteLine("\nEnter Type - 0 for Medical, 1 for Technical, 2 for Social, 3 for Transportation");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                throw new BlInvalidException("\nType cannot be null or empty");
            if (!int.TryParse(input, out int typeNumber))
                throw new BlInvalidException("\nInput must be a number");
            if (!Enum.IsDefined(typeof(BO.CallType), typeNumber))
                throw new BlInvalidException("\nInvalid type entered");
            BO.CallType type = (BO.CallType)typeNumber;
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                throw new BlInvalidException("\n Description cannot be null or empty");
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new BlInvalidException("\n Address cannot be null or empty");
            Console.WriteLine("\nEnter max end time in format: dd/MM/yyyy HH:mm:ss");
            string endTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(endTimeInput))
                throw new BlInvalidException("\nEnd time cannot be null or empty.");
            if (!DateTime.TryParseExact(endTimeInput, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime maxEndTime))
                throw new BlInvalidException("\nInvalid end time format. Expected format: dd/MM/yyyy HH:mm:ss.");

            var call = new BO.Call
            {
                Type = type,
                Description = description,
                CallerAddress = address,
                MaxEndTime = maxEndTime,
            };

            s_bl.Call.Create(call);
            Console.WriteLine("Call added successfully.");
        }
        // Retrieves and displays a call's information by ID.
        private static void GetCall()
        {
            Console.Write("Enter Call Id: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }

            var call = s_bl.Call.GetCallDetails(id);
            Console.WriteLine(call);
        }
        // Updates an existing call's information.
        private static void UpdateCall()
        {
            Console.Write("Enter Call Id to Update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }
            var call = s_bl.Call.Read(id);

            Console.WriteLine("\nEnter Type - 0 for Medical, 1 for Technical, 2 for Social, 3 for Transportation");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                throw new BlInvalidException("\nType cannot be null or empty");
            if (!int.TryParse(input, out int typeNumber))
                throw new BlInvalidException("\nInput must be a number");
            if (!Enum.IsDefined(typeof(BO.CallType), typeNumber))
                throw new BlInvalidException("\nInvalid type entered");
            BO.CallType type = (BO.CallType)typeNumber;
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                throw new BlInvalidException("\n Description cannot be null or empty");
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new BlInvalidException("\n Address cannot be null or empty");
            Console.WriteLine("\nEnter max end time in format: dd/MM/yyyy HH:mm:ss");
            string endTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(endTimeInput))
                throw new BlInvalidException("\nEnd time cannot be null or empty.");
            if (!DateTime.TryParseExact(endTimeInput, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime maxEndTime))
                throw new BlInvalidException("\nInvalid end time format. Expected format: dd/MM/yyyy HH:mm:ss.");
            Console.WriteLine("\nEnter CallStatus - 0 for New, 1 for Open, 2 for InProgress, 3 for Resolved, 4 for Closed, 5 for Expired, 6 for Aborted");
            string statusInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(statusInput))
                throw new BlInvalidException("\nCall status cannot be null or empty");
            if (!int.TryParse(statusInput, out int statusNumber))
                throw new BlInvalidException("\nInput must be a number");
            if (!Enum.IsDefined(typeof(BO.CallStatus), statusNumber))
                throw new BlInvalidException("\nInvalid status entered");
            BO.CallStatus callStatus = (BO.CallStatus)statusNumber;

            s_bl.Call.Update(new BO.Call
            {
                Id = id,
                Type = (BO.CallType)call.Type,
                Description = description,
                CallerAddress = address,
                StartTime = call.StartTime,
                MaxEndTime = maxEndTime,
                Status = callStatus
            });
            Console.WriteLine("Call updated successfully.");
        }
        // Removes a call from the system by ID.
        private static void RemoveCall()
        {
            Console.Write("Enter Call Id to Remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid Id.");
                return;
            }
            s_bl.Call.Delete(id);
            Console.WriteLine("Call removed successfully.");
        }
    }
}
