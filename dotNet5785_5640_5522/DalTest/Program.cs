using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic.FileIO;
namespace DalTest;
internal class Program
{
    static readonly IDal s_dal = new DalList(); //stage 2
    private enum MainMenu
    {
        exit,
        volunteers,
        calls,
        assignments,
        initializingData,
        displayData,
        config,
        resetData
    }
    private enum SubMenu
    {
        exit,
        create,
        read,
        readAll,
        update,
        delete,
        deleteAll
    }
    private enum AddConfigMenu
    {
        get,
        create,
        add
    }
    private enum DisplayConfigMenu
    {
        getId,
        DisplayFuncion,
        Display
    }
    private enum SubConfigMenu
    {
        exit,
        minute,
        hour,
        day,
        month,
        year,
        displayClockSystem,
        redefine,
        displayClockConfig,
        reset
    }
    // Main entry point; displays the main menu, handles user input, and routes to corresponding functionality.
    static void Main(string[] args)
    {
        try
        {
            MainMenu option;
            do
            {
                Console.WriteLine("\n MainMenu is on");
                foreach (var options in Enum.GetValues(typeof(MainMenu)))
                {
                    Console.WriteLine($"\n Enter {(int)options} to {options.ToString().Replace('_', ' ').ToLower()}");
                }
                option = (MainMenu)GetIntFromScreen("\n Enter your choice: ");
                Console.Clear();
                switch (option)
                {
                    case MainMenu.exit:
                        Console.WriteLine("\n Have a good day!");
                        break;
                    case MainMenu.volunteers:
                        MenuFanction("volunteers");
                        break;
                    case MainMenu.calls:
                        MenuFanction("calls");
                        break;
                    case MainMenu.assignments:
                        MenuFanction("Assignments");
                        break;
                    case MainMenu.initializingData:
                        InitializingDataFunction();
                        break;
                    case MainMenu.displayData:
                        DisplayDataFunction();
                        break;
                    case MainMenu.config:
                        ConfigMenuFunction();
                        break;
                    case MainMenu.resetData:
                        ResetDataFunction();
                        break;
                    default:
                        throw new DalInvalidException("\n Ther is no option like this");
                }
            }
            while (option != MainMenu.exit);
        }
        catch (Exception e)
        {
            Console.WriteLine($"\n error: {e.ToString()}");
        }
    }
    // Handles the submenu for CRUD operations based on the specified type (e.g., volunteers, calls, assignments).
    private static void MenuFanction(string typeOf)
    {
        SubMenu option;
        Console.WriteLine("\n SubMenu is on");
        do
        {
            foreach (var myOption in Enum.GetValues(typeof(SubMenu)))
            {
                Console.WriteLine($"\n Enter {(int)myOption} to {myOption.ToString().Replace('_', ' ').ToLower()}");
            }
            option = (SubMenu)GetIntFromScreen("\n Enter your choice: ");
            switch (option)
            {
                case SubMenu.create:
                    CreateFunction(typeOf);
                    break;
                case SubMenu.read:
                    ReadFunction(typeOf);
                    break;
                case SubMenu.readAll:
                    ReadAllFunction(typeOf);
                    break;
                case SubMenu.update:
                    UpdateFunction(typeOf);
                    break;
                case SubMenu.delete:
                    DeleteFunction(typeOf);
                    break;
                case SubMenu.deleteAll:
                    DeleteAllFunction(typeOf);
                    break;
                default:
                    throw new DalInvalidException("\n There is no option like this");
            }
        }
        while (option != SubMenu.exit);
    }
    // Initializes data by calling the `Initialization.Do` method with the appropriate data sources.
    private static void InitializingDataFunction()
    {
        Initialization.Do(s_dal); // stage 2
    }
    // Displays all data of a specified type (volunteer, call, or assignment) by reading from corresponding data sources.
    private static void DisplayDataFunction()
    {
        Console.WriteLine("\n Enter type of config (volunteer, call, assignment)");
        string typeOf = Console.ReadLine();
        switch (typeOf)
        {
            case "volunteer":
                if (s_dal.Volunteer != null) // stage 2
                {
                    var volunteers = s_dal.Volunteer.ReadAll(); //stage 2
                    foreach (var volunteer in volunteers)
                        Console.WriteLine(volunteer);
                }
                else throw new Exception("\n s_dalVolunteer is null");
                break;
            case "call":
                if (s_dal.Call != null) //stage 2
                {
                    var calls = s_dal.Call.ReadAll(); //stage 2
                    foreach (var call in calls)
                        Console.WriteLine(call);
                }
                else throw new Exception("\n s_dalCall is null");
                break;
            case "assignment":
                if (s_dal.Assignment != null) //stage 2
                {
                    var assignments = s_dal.Assignment.ReadAll(); //stage 2
                    foreach (var assignment in assignments)
                        Console.WriteLine(assignment); 
                }
                else throw new Exception("\n s_dalAssignment is null");
                break;
            default:
                throw new DalInvalidException($"\n There is no type of config like {typeOf}");
        }
    }
    // Handles the configuration submenu to manage time settings, display clock details, or reset configurations.
    private static void ConfigMenuFunction()
    {
        SubConfigMenu option;
        do
        {
            Console.WriteLine("\n SubConfigMenu is on");
            foreach (var myOption in Enum.GetValues(typeof(SubConfigMenu)))
            {
                Console.WriteLine($"\n Enter {(int)myOption} to {myOption.ToString().Replace('_', ' ').ToLower()}");
            }
            option = (SubConfigMenu)GetIntFromScreen("\n Enter your choice: ");
            Console.Clear();
            switch (option)
            {
                case SubConfigMenu.exit:
                    Console.WriteLine("\n Have a good day!");
                    break;
                case SubConfigMenu.minute:
                    ChangeTimeFunction("minute");
                    break;
                case SubConfigMenu.hour:
                    ChangeTimeFunction("hour");
                    break;
                case SubConfigMenu.day:
                    ChangeTimeFunction("day");
                    break;
                case SubConfigMenu.month:
                    ChangeTimeFunction("month");
                    break;
                case SubConfigMenu.year:
                    ChangeTimeFunction("year");
                    break;
                case SubConfigMenu.displayClockSystem:
                    DisplayClockSystemFunction();
                    break;
                case SubConfigMenu.redefine:
                    RedefineFunction();
                    break;
                case SubConfigMenu.displayClockConfig:
                    DisplayClockConfigFunction();
                    break;
                case SubConfigMenu.reset:
                    ResetFunction();
                    break;
                default:
                    throw new DalInvalidException("\n Ther is no option like this");
            }
        }
        while (option != SubConfigMenu.exit);
    }
    // Resets all data by clearing all DAL objects (volunteer, call, assignment) or throwing an exception if any is null.
    private static void ResetDataFunction()
    {
        if (s_dal.Config != null)  //stage 2
            s_dal.Config.reset(); //stage 2
        else throw new DalDependencyNotInitializedException("\n s_dalConfig is null");

        if (s_dal.Assignment != null) //stage 2
            s_dal.Assignment.DeleteAll(); //stage 2
        else throw new DalDependencyNotInitializedException("\n s_dalAssignment is null");

        if (s_dal.Call != null) //stage 2
            s_dal.Call.DeleteAll(); //stage 2
        else throw new DalDependencyNotInitializedException("\n s_dalCall is null");

        if (s_dal.Volunteer != null) //stage 2
            s_dal.Volunteer.DeleteAll(); //stage 2
        else throw new DalDependencyNotInitializedException("\n s_dalVolunteer is null");
    }
    // Creates and adds a new Volunteer, Call, or Assignment object based on user input.
    private static void CreateFunction(string typeOf)
    {
        if (typeOf == "volunteers")
        {
            Console.WriteLine("\n Enter ID");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("\n Enter valid input");
            Console.WriteLine("\n Enter first name");
            string firstName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DalInvalidException("\n First name cannot be null or empty");
            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DalInvalidException("\n Last name cannot be null or empty");
            Console.WriteLine("\n Enter phone number");
            string phoneNumber = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DalInvalidException("\n Phone number cannot be null or empty");
            Console.WriteLine("\n Enter email address");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
                throw new DalInvalidException("\n Email address cannot be null or empty");
            Console.WriteLine("\n Enter password");
            string password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(password))
                throw new DalInvalidException("\n Password cannot be null or empty");
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new DalInvalidException("\n Address cannot be null or empty");
            Console.WriteLine("\n Enter Latitude");
            double latitude;
            if (!double.TryParse(Console.ReadLine(), out latitude))
                throw new DalInvalidException("\n Invalid input for Latitude");
            Console.WriteLine("\n Enter Longitude");
            double longitude;
            if (!double.TryParse(Console.ReadLine(), out longitude))
                throw new DalInvalidException("\n Invalid input for Longitude");
            Volunteer volunteer = new Volunteer(id, firstName, lastName, phoneNumber, email, password, address, latitude, longitude);
            s_dal.Volunteer.Create(volunteer); // stage 2
        }
        else if (typeOf == "calls")
        {
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                throw new DalInvalidException("\n Description cannot be null or empty");
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new DalInvalidException("\n Address cannot be null or empty");
            Console.WriteLine("\n Enter Latitude");
            double latitude;
            if (!double.TryParse(Console.ReadLine(), out latitude))
                throw new DalInvalidException("\n Invalid input for Latitude");
            Console.WriteLine("\n Enter Longitude");
            double longitude;
            if (!double.TryParse(Console.ReadLine(), out longitude))
                throw new DalInvalidException("\n invalid input for Longitude");
            Call call = new Call(description, address, latitude, longitude);
            s_dal.Call.Create(call); // stage 2
        }
        else if (typeOf == "Assignments")
        {
            Console.WriteLine("\n Enter volunteer id");
            string volunteerIdInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(volunteerIdInput))
                throw new DalInvalidException("\n Volunteer id cannot be null or empty");
            int volunteerId;
            if (!int.TryParse(volunteerIdInput, out volunteerId))
                Console.WriteLine("\n Enter volunteer id");
            Console.WriteLine("\n Enter call id");
            string callIdInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(callIdInput))
                throw new DalInvalidException("\n Call id cannot be null or empty");
            int callId;
            if (!int.TryParse(callIdInput, out callId))
                throw new DalInvalidException("\n Call id must be a valid number");
            Console.WriteLine("\n Enter entry time (yyyy-MM-dd HH:mm)");
            string entryTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entryTimeInput))
                throw new DalInvalidException("\n Entry time cannot be null or empty");
            DateTime entryTime;
            if (!DateTime.TryParse(entryTimeInput, out entryTime))
                throw new DalInvalidException("\n Invalid entry time format");
            Console.WriteLine("\n Enter end time (yyyy-MM-dd HH:mm)");
            string endTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(endTimeInput))
                throw new DalInvalidException("\n End time cannot be null or empty");
            DateTime endTime;
            if (!DateTime.TryParse(endTimeInput, out endTime))
                throw new DalInvalidException("\n Invalid end time format");
            Assignment assignment = new Assignment(volunteerId, callId, entryTime, endTime);
            s_dal.Assignment.Create(assignment); // stage 2
        }
        else throw new DalInvalidException("\n There is no option like this");
    }
    // Reads and displays a specific Volunteer, Call, or Assignment object by ID.
    private static void ReadFunction(string typeOf)
    {
        Console.WriteLine("Enter ID for display");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input");
        if (typeOf == "volunteers")
            if (s_dal.Volunteer != null) // stage 2
                Console.WriteLine(s_dal.Volunteer.Read(id)); // stage 2
            else throw new DalDependencyNotInitializedException("s_dal.Volunteer is null");
        else if (typeOf == "calls")
            if (s_dal.Call != null) // stage 2
                Console.WriteLine(s_dal.Call.Read(id)); // stage 2
            else throw new DalDependencyNotInitializedException("s_dal.Volunteer is null");
        else if (typeOf == "assingments")
            if (s_dal.Assignment != null) // stage 2
                    Console.WriteLine(s_dal.Assignment.Read(id)); // stage 2
            else throw new DalDependencyNotInitializedException("s_dal.Assignment is null");
        else throw new DalInvalidException("\n There is no option like this");
    }
    // Reads and displays all Volunteers, Calls, or Assignments or indicates if no data exists.
    private static void ReadAllFunction(string typeOf)
    {
        IEnumerable<object> list;

        if (typeOf == "volunteers")
            if (s_dal.Volunteer != null) // stage 2
                list = s_dal.Volunteer.ReadAll(); // stage 2
            else throw new DalDependencyNotInitializedException("s_dal.Volunteer is null");

        else if (typeOf == "calls")
            if (s_dal.Call != null) // stage 2
                list = s_dal.Call.ReadAll(); // stage 2
            else throw new DalDependencyNotInitializedException("s_dal.Call is null");

        else if (typeOf == "Assignments")
            if (s_dal.Assignment != null) // stage 2
                list = s_dal.Assignment.ReadAll(); // stage 2
            else throw new DalDependencyNotInitializedException("s_dal.Assignment is null");
        else
            throw new DalInvalidException("\n There is no option like this");
        if (list == null || !list.Any())
            Console.WriteLine($"\n There are no {typeOf} for display");
        else
            foreach (var item in list)
                Console.WriteLine(($"\n{typeOf} List:\n {item}"));
    }
    // Updates an existing Volunteer, Call, or Assignment object based on user input.
    private static void UpdateFunction(string typeOf)
    {
        if (typeOf == "volunteers")
        {
            Console.WriteLine("\n Enter ID");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("\n Enter valid input");
            Console.WriteLine("\n Enter first name");
            string firstName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DalInvalidException("First name cannot be null or empty");
            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DalInvalidException("Last name cannot be null or empty");
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
            Console.WriteLine("\n Enter Latitude");
            double latitude;
            while (!double.TryParse(Console.ReadLine(), out latitude))
                Console.WriteLine("\n Enter valid Latitude");
            Console.WriteLine("\n Enter Longitude");
            double longitude;
            while (!double.TryParse(Console.ReadLine(), out longitude))
                Console.WriteLine("\n Enter valid Longitude");
            Volunteer volunteer = new Volunteer(id, firstName, lastName, phoneNumber, email, password, address, latitude, longitude);
            s_dal.Volunteer.Update(volunteer); // stage 2
        }
        else if (typeOf == "calls")
        {
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                throw new DalInvalidException("Description cannot be null or empty");
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new DalInvalidException("Address cannot be null or empty");
            Console.WriteLine("\n Enter Latitude");
            double latitude;
            while (!double.TryParse(Console.ReadLine(), out latitude))
                Console.WriteLine("\n Enter valid Latitude");
            Console.WriteLine("\n Enter Longitude");
            double longitude;
            while (!double.TryParse(Console.ReadLine(), out longitude))
                Console.WriteLine("\n Enter valid Longitude");
            Call call = new Call(description, address, latitude, longitude);
            s_dal.Call.Update(call); // stage 2
        }
        else if (typeOf == "Assignments")
        {
            Console.WriteLine("\n Enter volunteer id");
            string volunteerIdInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(volunteerIdInput))
                throw new DalInvalidException("\n Volunteer id cannot be null or empty");
            int volunteerId;
            if (!int.TryParse(volunteerIdInput, out volunteerId))
                throw new DalInvalidException("\n Volunteer id must be a valid number");
            Console.WriteLine("\n Enter call id");
            string callIdInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(callIdInput))
                throw new DalInvalidException("\n Call id cannot be null or empty");
            int callId;
            if (!int.TryParse(callIdInput, out callId))
                throw new DalInvalidException("\n Call id must be a valid number");
            Console.WriteLine("\n Enter entry time (yyyy-MM-dd HH:mm)");
            string entryTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entryTimeInput))
                throw new DalInvalidException("\n Entry time cannot be null or empty");
            DateTime entryTime;
            if (!DateTime.TryParse(entryTimeInput, out entryTime))
                throw new DalInvalidException("\n Invalid entry time format");
            Console.WriteLine("\n Enter end time (yyyy-MM-dd HH:mm)");
            string endTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(endTimeInput))
                throw new DalInvalidException("\n End time cannot be null or empty");
            DateTime endTime;
            if (!DateTime.TryParse(endTimeInput, out endTime))
                throw new DalInvalidException("\n Invalid end time format");
            Assignment assignment = new Assignment(volunteerId, callId, entryTime, endTime);
            s_dal.Assignment.Update(assignment); // stage 2
        }
        else throw new DalInvalidException("\n There is no option like this");
    }
    private static void DeleteFunction(string typeOf)
    {
        Console.WriteLine("\n Enter ID");
        int id;
        string input;
        do
        {
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                Console.WriteLine("\n ID cannot be null or empty. Please enter a valid ID:");
        }
        while (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out id));
        if (typeOf == "volunteers")
            if (s_dal.Volunteer != null) //stage 2
                s_dal.Volunteer.Delete(id); //stage 2
            else throw new DalDependencyNotInitializedException("\n s_dal.Volunteer is null");
        else if (typeOf == "calls")
            if (s_dal.Call != null) //stage 2
                s_dal.Call.Delete(id); //stage 2
            else throw new DalDependencyNotInitializedException("\n s_dal.Call is null");
        else if (typeOf == "assignments")
            if (s_dal.Assignment != null) //stage 2
                s_dal.Assignment.Delete(id); //stage 2
            else throw new DalDependencyNotInitializedException("\n s_dal.Assignment is null");
        else
            throw new DalInvalidException("\n There is no option like this");
    }
    // Deletes all entities of a specified type.
    private static void DeleteAllFunction(string typeOf)
    {
        if (typeOf == "volunteers")
            s_dal.Volunteer.DeleteAll(); //stage 2
        else if (typeOf == "calls")
            s_dal.Call.DeleteAll(); // stage 2
        else if (typeOf == "assignments")
            s_dal.Assignment.DeleteAll(); //stage 2
        else throw new DalInvalidException("\n There is no option like this");
    }
    // Adjusts the system clock by the specified time unit.
    private static void ChangeTimeFunction(string typeOf)
    {
        if (s_dal.Config != null) // stage 2
        {
            if (typeOf == "minute")
                s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1); //stage 2
            else if (typeOf == "hour")
                s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1); //stage 2
            else if (typeOf == "day")
                s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1); //stage 2
            else if (typeOf == "month")
                s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1); //stage 2
            else if (typeOf == "year")
                s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1); //stage 2
            else throw new DalInvalidException("\n There is no option like this");
        }
        else throw new DalDependencyNotInitializedException("s_dal.Config is null");
    }
    // Displays the current system clock value.
    private static void DisplayClockSystemFunction()
    {
        if (s_dal.Config != null) //stage 2
            Console.WriteLine($"The system clock is: {s_dal.Config.Clock}");
        else throw new DalDependencyNotInitializedException("s_dal.Config is null");
    }
    // Redefines the attributes of a specific entity based on user input.
    private static void RedefineFunction()
    {
        Console.WriteLine("\n Enter type of config (volunteer, call, assignment):");
        string typeOf = Console.ReadLine();
        Console.WriteLine("\n Enter ID:");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input:");
        string result;
        switch (typeOf.ToLower())
        {
            case "volunteer":
                Console.WriteLine("\n Enter field to change (first name, last name, address, phone number, email):");
                string volunteerOption = Console.ReadLine()?.ToLower();
                var volunteerToChange = s_dal.Volunteer.Read(id); // stage 2
                if (volunteerToChange == null)
                    throw new DalDependencyNotInitializedException("\n Volunteer not found.");
                switch (volunteerOption)
                {
                    case "first name":
                        Console.Write("\n Enter First Name: ");
                        result = Console.ReadLine();
                        volunteerToChange = volunteerToChange with { FirstName = result };
                        break;
                    case "last name":
                        Console.Write("\n Enter Last Name: ");
                        result = Console.ReadLine();
                        volunteerToChange = volunteerToChange with { LastName = result };
                        break;
                    case "address":
                        Console.Write("\n Enter Address: ");
                        result = Console.ReadLine();
                        volunteerToChange = volunteerToChange with { Address = result };
                        break;
                    case "phone number":
                        Console.Write("\n Enter Phone Number: ");
                        result = Console.ReadLine();
                        volunteerToChange = volunteerToChange with { PhoneNumber = result };
                        break;
                    case "email":
                        Console.Write("\n Enter Email: ");
                        result = Console.ReadLine();
                        volunteerToChange = volunteerToChange with { Email = result };
                        break;
                    default:
                        throw new DalInvalidException("\n Invalid input.");
                }
                s_dal.Volunteer.Update(volunteerToChange); // stage 2
                Console.WriteLine("\n Volunteer updated successfully.");
                break;
            case "call":
                Console.WriteLine("\n Enter field to change (description, address):");
                string callOption = Console.ReadLine()?.ToLower();
                var callToChange = s_dal.Call.Read(id); // stage 2
                if (callToChange == null)
                    throw new DalDependencyNotInitializedException("\n Call not found.");

                switch (callOption)
                {
                    case "description":
                        Console.Write("\n Enter Description: ");
                        result = Console.ReadLine();
                        callToChange = callToChange with { Description = result };
                        break;
                    case "address":
                        Console.Write("\n Enter Address: ");
                        result = Console.ReadLine();
                        callToChange = callToChange with { CallerAddress = result };
                        break;
                    default:
                        throw new DalInvalidException("\n Invalid input.");
                }

                s_dal.Call.Update(callToChange); // stage 2
                Console.WriteLine("\n Call updated successfully.");
                break;
            case "assignment":
                Console.WriteLine("\n Enter field to change (volunteer id, entry time, end time):");
                string assignmentOption = Console.ReadLine()?.ToLower();
                var assignmentToChange = s_dal.Assignment.Read(id); // stage 2
                if (assignmentToChange == null)
                    throw new DalDependencyNotInitializedException("\n Call not found.");
                switch (assignmentOption)
                {
                    case "volunteer id":
                        Console.Write("\n Enter Volunteer id: ");
                        result = Console.ReadLine();
                        if (!int.TryParse(result, out int volunteerId))
                            throw new DalInvalidException("\n Invalid Volunteer id.");
                        assignmentToChange = assignmentToChange with { VolunteerId = volunteerId };
                        break;
                    case "entry time":
                        Console.Write("\n Enter Entry time (yyyy-mm-dd hh:mm): ");
                        result = Console.ReadLine();
                        if (!DateTime.TryParse(result, out DateTime entryTime))
                            throw new DalInvalidException("\n Invalid Entry time.");
                        assignmentToChange = assignmentToChange with { ArrivalTime = entryTime };
                        break;
                    case "end time":
                        Console.Write("\n Enter End time (yyyy-mm-dd hh:mm): ");
                        result = Console.ReadLine();
                        if (!DateTime.TryParse(result, out DateTime endTime))
                            throw new DalInvalidException("\n Invalid End time.");
                        assignmentToChange = assignmentToChange with { EndTime = endTime };
                        break;
                    default:
                        throw new DalInvalidException("\n Invalid input.");
                }
                s_dal.Assignment.Update(assignmentToChange); // stage 2
                Console.WriteLine("\n Assignment updated successfully.");
                break;
            default:
                throw new DalInvalidException($"\n No such type of config: {typeOf}");
        }
    }
    // Displays the configuration clock.
    private static void DisplayClockConfigFunction()
    {
        if (s_dal.Config != null) // stage 2
            Console.WriteLine(s_dal.Config.Clock);
        else throw new DalDependencyNotInitializedException("\n s_dal.Config is null");
    }
    // Resets the system configuration.
    private static void ResetFunction()
    {
        if (s_dal.Config != null) // stage 2
            s_dal.Config.reset(); // stage 2
        else throw new DalDependencyNotInitializedException("\n s_dal.Config is null");
    }
    // Retrieves an integer value from the user with validation.
    private static int GetIntFromScreen(string message)
    {
        Console.Write(message);
        if (!int.TryParse(Console.ReadLine(), out int result))
            throw new DalInvalidException("\n Invalid input");
        return result;
    }
}