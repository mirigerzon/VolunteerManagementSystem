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
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
    private static ICall? s_dalCall = new CallImplementation();
    private static IAssignment? s_dalAssignment = new AssignmentImplementation();
    private static IConfig? s_dalConfig = new ConfigImplementation();
    private enum MainMenu
    {
        exit, volunteers, calls, assignments, initializingData, displayData, config, resetData
    }
    private enum SubMenu
    {
        exit, create, read, readAll, update, delete, deleteAll
    }
    private enum AddConfigMenu
    {
        get, create, add
    }
    private enum DisplayConfigMenu
    {
        getId, DisplayFuncion, Display
    }
    private enum SubConfigMenu
    {
        exit, minute, hour, day, month, year, displayClockSystem, redefine, displayClockConfig, reset
    }
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
                        throw new Exception("\n Ther is no option like this");
                }
            }
            while (option != MainMenu.exit);
        }
        catch (Exception e)
        {
            Console.WriteLine($"\n error: {e.ToString()}");
        }
    }
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
                    throw new Exception("\n Error");
            }
        }
        while (option != SubMenu.exit);
        return;
    }
    private static void InitializingDataFunction()
    {
        Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
    }
    private static void DisplayDataFunction()
    {
        Console.WriteLine("\n Enter type of config (volunteer, call, assignment)");
        string typeOf = Console.ReadLine();
        Console.WriteLine("\n Enter ID");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input");
        switch (typeOf)
        {
            case "volunteer":
                if (s_dalVolunteer != null)
                    Console.WriteLine(s_dalVolunteer.Read(id));
                else throw new Exception("\n s_dalVolunteer is null");
                break;
            case "call":
                if (s_dalCall != null)
                    Console.WriteLine(s_dalCall.Read(id));
                else throw new Exception("\n s_dalCall is null");
                break;
            case "assignment":
                if (s_dalAssignment != null)
                    Console.WriteLine(s_dalAssignment.Read(id));
                else throw new Exception("\n s_dalAssignment is null");
                break;
            default:
                throw new Exception($"\n Ther is no type of config like {typeOf}");
        }
    }
    private static void ConfigMenuFunction()
    {
        MainMenu option;
        do
        {
            Console.WriteLine("\n SubConfigMenu is on");
            foreach (var myOption in Enum.GetValues(typeof(MainMenu)))
            {
                Console.WriteLine($"\n Enter {(int)myOption} to {myOption.ToString().Replace('_', ' ').ToLower()}");
            }
            option = (MainMenu)GetIntFromScreen("\n Enter your choice: ");
            Console.Clear();
            switch (option)
            {
                case (MainMenu)SubConfigMenu.exit:
                    Console.WriteLine("\n Have a good day!");
                    break;
                case (MainMenu)SubConfigMenu.minute:
                    changeTimeFunction("minute");
                    break;
                case (MainMenu)SubConfigMenu.hour:
                    changeTimeFunction("hour");
                    break;
                case (MainMenu)SubConfigMenu.day:
                    changeTimeFunction("day");
                    break;
                case (MainMenu)SubConfigMenu.month:
                    changeTimeFunction("month");
                    break;
                case (MainMenu)SubConfigMenu.year:
                    changeTimeFunction("year");
                    break;
                case (MainMenu)SubConfigMenu.displayClockSystem:
                    DisplayClockSystemFunction();
                    break;
                case (MainMenu)SubConfigMenu.redefine:
                    RedefineFunction();
                    break;
                case (MainMenu)SubConfigMenu.displayClockConfig:
                    DisplayClockConfigFunction();
                    break;
                case (MainMenu)SubConfigMenu.reset:
                    ResetFunction();
                    break;
                default:
                    throw new Exception("\n Ther is no option like this");
            }
        }
        while (option != MainMenu.exit);
    }
    private static void ResetDataFunction()
    {
        if (s_dalConfig != null)
            s_dalConfig.reset();
        else throw new Exception("\n s_dalConfig is null");
        if (s_dalAssignment != null)
            s_dalAssignment.DeleteAll();
        else throw new Exception("\n s_dalAssignment is null");
        if (s_dalCall != null)
            s_dalCall.DeleteAll();
        else throw new Exception("\n s_dalCall is null");
        if (s_dalVolunteer != null)
            s_dalVolunteer.DeleteAll();
        else throw new Exception("\n s_dalVolunteer is null");
    }
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
                throw new Exception("\n First name cannot be null or empty");
            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName))
                throw new Exception("\n Last name cannot be null or empty");
            Console.WriteLine("\n Enter phone number");
            string phoneNumber = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new Exception("\n Phone number cannot be null or empty");
            Console.WriteLine("\n Enter email address");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("\n Email address cannot be null or empty");
            Console.WriteLine("\n Enter password");
            string password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("\n Password cannot be null or empty");
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("\n Address cannot be null or empty");
            Console.WriteLine("\n Enter Latitude");
            double latitude;
            if (!double.TryParse(Console.ReadLine(), out latitude))
                throw new Exception("\n Invalid input for Latitude");
            Console.WriteLine("\n Enter Longitude");
            double longitude;
            if (!double.TryParse(Console.ReadLine(), out longitude))
                throw new Exception("\n Invalid input for Longitude");
            Volunteer volunteer = new Volunteer(id, firstName, lastName, phoneNumber, email, password, address, latitude, longitude);
            s_dalVolunteer.Create(volunteer);
        }
        else if (typeOf == "calls")
        {
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                throw new Exception("\n Description cannot be null or empty");

            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("\n Address cannot be null or empty");

            Console.WriteLine("\n Enter Latitude");
            double latitude;
            if (!double.TryParse(Console.ReadLine(), out latitude))
                throw new Exception("\n Invalid input for Latitude");

            Console.WriteLine("\n Enter Longitude");
            double longitude;
            if (!double.TryParse(Console.ReadLine(), out longitude))
                throw new Exception("\n invalid input for Longitude");

            Call call = new Call(description, address, latitude, longitude);
            s_dalCall.Create(call);
        }
        else if (typeOf == "assignments")
        {
            Console.WriteLine("\n Enter volunteer id");
            string volunteerIdInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(volunteerIdInput))
                throw new Exception("\n Volunteer id cannot be null or empty");

            int volunteerId;
            if (!int.TryParse(volunteerIdInput, out volunteerId))
                throw new Exception("\n Volunteer id must be a valid number");

            Console.WriteLine("\n Enter entry time (yyyy-MM-dd HH:mm)");
            string entryTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entryTimeInput))
                throw new Exception("\n Entry time cannot be null or empty");

            DateTime entryTime;
            if (!DateTime.TryParse(entryTimeInput, out entryTime))
                throw new Exception("\n Invalid entry time format");

            Console.WriteLine("\n Enter end time (yyyy-MM-dd HH:mm)");
            string endTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(endTimeInput))
                throw new Exception("\n End time cannot be null or empty");

            DateTime endTime;
            if (!DateTime.TryParse(endTimeInput, out endTime))
                throw new Exception("\n Invalid end time format");

            Assignment assignment = new Assignment(volunteerId, entryTime, endTime);
            s_dalAssignment.Create(assignment);
        }

        else throw new Exception("\n Error");
    }
    private static void ReadFunction(string typeOf)
    {
        Console.WriteLine("Enter ID for display");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input");
        if (typeOf == "volunteers")
            if (s_dalVolunteer != null)
                Console.WriteLine(s_dalVolunteer.Read(id));
            else throw new Exception("s_dalVolunteer is null");
        else if (typeOf == "calls")
            if (s_dalCall != null)
                Console.WriteLine(s_dalCall.Read(id));
            else throw new Exception("s_dalVolunteer is null");
        else if (typeOf == "assingments")
            if (s_dalAssignment != null)
                Console.WriteLine(s_dalAssignment.Read(id));
            else throw new Exception("s_dalAssignment is null");
        else throw new Exception("Error");
    }
    private static void ReadAllFunction(string typeOf)
    {
        IEnumerable<object> list;

        if (typeOf == "volunteers")
            if (s_dalVolunteer != null)
                list = s_dalVolunteer.ReadAll();
            else throw new Exception("s_dalVolunteer is null");
        else if (typeOf == "calls")
            if (s_dalCall != null)
                list = s_dalCall.ReadAll();
            else throw new Exception("s_dalCall is null");
        else if (typeOf == "assignments")
            if (s_dalAssignment != null)
                list = s_dalAssignment.ReadAll();
            else throw new Exception("s_dalAssignment is null");
        else
            throw new Exception("ERROR");
        if (list == null || !list.Any())
            Console.WriteLine($"\n There are no {typeOf} for display");
        else
            foreach (var item in list)
                Console.WriteLine(($"\n{typeOf} List:\n {item}"));
    }
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
                throw new Exception("First name cannot be null or empty");

            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(lastName))
                throw new Exception("Last name cannot be null or empty");

            Console.WriteLine("\n Enter phoneNumber");
            string phoneNumber = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new Exception("Phone number cannot be null or empty");

            Console.WriteLine("\n Enter email address");
            string email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email address cannot be null or empty");

            Console.WriteLine("\n Enter password");
            string password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password cannot be null or empty");

            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("Address cannot be null or empty");

            Console.WriteLine("\n Enter Latitude");
            double latitude;
            while (!double.TryParse(Console.ReadLine(), out latitude))
                Console.WriteLine("\n Enter valid Latitude");

            Console.WriteLine("\n Enter Longitude");
            double longitude;
            while (!double.TryParse(Console.ReadLine(), out longitude))
                Console.WriteLine("\n Enter valid Longitude");

            Volunteer volunteer = new Volunteer(id, firstName, lastName, phoneNumber, email, password, address, latitude, longitude);
            s_dalVolunteer.Update(volunteer);
        }
        else if (typeOf == "calls")
        {
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                throw new Exception("Description cannot be null or empty");

            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("Address cannot be null or empty");

            Console.WriteLine("\n Enter Latitude");
            double latitude;
            while (!double.TryParse(Console.ReadLine(), out latitude))
                Console.WriteLine("\n Enter valid Latitude");

            Console.WriteLine("\n Enter Longitude");
            double longitude;
            while (!double.TryParse(Console.ReadLine(), out longitude))
                Console.WriteLine("\n Enter valid Longitude");

            Call call = new Call(description, address, latitude, longitude);
            s_dalCall.Update(call);
        }
        else if (typeOf == "assignments")
        {
            Console.WriteLine("\n Enter volunteer id");
            string volunteerIdInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(volunteerIdInput))
                throw new Exception("\n Volunteer id cannot be null or empty");

            int volunteerId;
            if (!int.TryParse(volunteerIdInput, out volunteerId))
                throw new Exception("\n Volunteer id must be a valid number");

            Console.WriteLine("\n Enter entry time (yyyy-MM-dd HH:mm)");
            string entryTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entryTimeInput))
                throw new Exception("\n Entry time cannot be null or empty");

            DateTime entryTime;
            if (!DateTime.TryParse(entryTimeInput, out entryTime))
                throw new Exception("\n Invalid entry time format");

            Console.WriteLine("\n Enter end time (yyyy-MM-dd HH:mm)");
            string endTimeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(endTimeInput))
                throw new Exception("\n End time cannot be null or empty");

            DateTime endTime;
            if (!DateTime.TryParse(endTimeInput, out endTime))
                throw new Exception("\n Invalid end time format");

            Assignment assignment = new Assignment(volunteerId, entryTime, endTime);
            s_dalAssignment.Update(assignment);
        }
        else throw new Exception("Error");
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
            if (s_dalVolunteer != null)
                s_dalVolunteer.Delete(id);
            else throw new Exception("\n s_dalVolunteer is null");
        else if (typeOf == "calls")
            if (s_dalCall != null)
                s_dalCall.Delete(id);
            else throw new Exception("\n s_dalCall is null");
        else if (typeOf == "assignments")
            if (s_dalAssignment != null)
                s_dalAssignment.Delete(id);
            else throw new Exception("\n s_dalAssignment is null");
        else
            throw new Exception("\n Error");
    }
    private static void DeleteAllFunction(string typeOf)
    {
        if (typeOf == "volunteers")
            s_dalVolunteer.DeleteAll();
        else if (typeOf == "calls")
            s_dalCall.DeleteAll();
        else if (typeOf == "assignments")
            s_dalAssignment.DeleteAll();
        else throw new Exception("\n Erroe");
    }
    private static void changeTimeFunction(string typeOf)
    {
        if (s_dalConfig != null)
        {
            if (typeOf == "minute")
                s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
            else if (typeOf == "hour")
                s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
            else if (typeOf == "day")
                s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
            else if (typeOf == "month")
                s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
            else if (typeOf == "year")
                s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
            else throw new Exception("\n Error");
        }
        else throw new Exception("s_dalConfig is null");

    }
    private static void DisplayClockSystemFunction()
    {
        if (s_dalConfig != null)
            Console.WriteLine($"The system clock is: {s_dalConfig.Clock}");
        else throw new Exception("s_dalConfig is null");
    }
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

                var volunteerToChange = s_dalVolunteer.Read(id);
                if (volunteerToChange == null)
                    throw new Exception("\n Volunteer not found.");

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
                        throw new Exception("\n Invalid input.");
                }

                s_dalVolunteer.Update(volunteerToChange);
                Console.WriteLine("\n Volunteer updated successfully.");
                break;

            case "call":
                Console.WriteLine("\n Enter field to change (description, address):");
                string callOption = Console.ReadLine()?.ToLower();

                var callToChange = s_dalCall.Read(id);
                if (callToChange == null)
                    throw new Exception("\n Call not found.");

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
                        throw new Exception("\n Invalid input.");
                }

                s_dalCall.Update(callToChange);
                Console.WriteLine("\n Call updated successfully.");
                break;

            case "assignment":
                Console.WriteLine("\n Enter field to change (volunteer id, entry time, end time):");
                string assignmentOption = Console.ReadLine()?.ToLower();

                var assignmentToChange = s_dalAssignment.Read(id);
                if (assignmentToChange == null)
                    throw new Exception("\n Call not found.");

                switch (assignmentOption)
                {
                    case "volunteer id":
                        Console.Write("\n Enter Volunteer id: ");
                        result = Console.ReadLine();
                        if (!int.TryParse(result, out int volunteerId))
                            throw new Exception("\n Invalid Volunteer id.");
                        assignmentToChange = assignmentToChange with { VolunteerId = volunteerId };
                        break;
                    case "entry time":
                        Console.Write("\n Enter Entry time (yyyy-mm-dd hh:mm): ");
                        result = Console.ReadLine();
                        if (!DateTime.TryParse(result, out DateTime entryTime))
                            throw new Exception("\n Invalid Entry time.");
                        assignmentToChange = assignmentToChange with { ArrivalTime = entryTime };
                        break;
                    case "end time":
                        Console.Write("\n Enter End time (yyyy-mm-dd hh:mm): ");
                        result = Console.ReadLine();
                        if (!DateTime.TryParse(result, out DateTime endTime))
                            throw new Exception("\n Invalid End time.");
                        assignmentToChange = assignmentToChange with { EndTime = endTime };
                        break;
                    default:
                        throw new Exception("\n Invalid input.");
                }

                s_dalAssignment.Update(assignmentToChange);
                Console.WriteLine("\n Assignment updated successfully.");
                break;

            default:
                throw new Exception($"\n No such type of config: {typeOf}");
        }
    }
    private static void DisplayClockConfigFunction()
    {
        if (s_dalConfig != null)
            Console.WriteLine(s_dalConfig.Clock);
        else throw new Exception("\n s_dalConfig is null");
    }
    private static void ResetFunction()
    {
        if (s_dalConfig != null)
            s_dalConfig.reset();
        else throw new Exception("\n s_dalConfig is null");
    }
    private static int GetIntFromScreen(string message)
    {
        Console.Write(message);
        if (!int.TryParse(Console.ReadLine(), out int result))
            throw new FormatException("\n Invalid input");
        return result;
    }
}