using System.Linq.Expressions;
using Dal;
using DalApi;
using DO;

namespace DalTest;

internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
    private static ICsll? s_dalCall = new CallImplementation();
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
                console.WriteLine("\n mainMenu are on");
                foreach (var option in Enum.GetValues(typeof(MainMenu)))
                {
                    Console.WriteLine($"\n Enter {(int)option} to {option.ToString().Replace('_', ' ').ToLower()}");
                }
                option = (MainMenu)GetIntFromUser("\n enter your choice: ");
                Console.Clear();
                switch (option)
                {
                    case MainMenu.exit:
                        Console.WriteLine("\n have a good day!");
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
                        throw ("\n ther is no option like this");
                        break;
                }
            }
            while (option != MainMenu.exit);
        }
        catch (Exception e)
        {
            Console.WriteLine($"error:" { e.ToString()});
        }
    }

    private static MenuFanction(type)
    {
        SubMenu option;
        console.WriteLine("\n SubMenu are on");
        do
        {
            foreach (var option in Enum.GetValues(typeof(SubMenu)))
            {
                Console.WriteLine($"Enter {(int)option} to {option.ToString().Replace('_', ' ').ToLower()}");
            }
            option = (MainMenu)GetIntFromUser("\n enter your choice: ");
            switch (option)
            {
                case SubMenu.create:
                    CreateFunction(type);
                    break;
                case SubMenu.read:
                    ReadFunction(type);
                    break;
                case SubMenu.readAll:
                    ReadAllFunction(type);
                    break;
                case SubMenu.update:
                    UpdateFunction(type);
                    break;
                case SubMenu.delete:
                    DeleteFunction(type);
                    break;
                case SubMenu.deleteAll:
                    DeleteAllFunction(type);
                    break;
                default:
                    Console.WriteLine("ERROR");
            }
        }
        while (option != SubMenu.exit);
    }
    private static InitializingDataFunction()
    {
        Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
    }
    private static DisplayDataFunction()
    {
        Console.WriteLine("\n enter type of config (volunteer, call, assignment)");
        string typeOf = Console.ReadLine();
        Console.WriteLine("\n enter ID");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input");
        switch (typeOf)
        {
            case "volunteer":
                Console.WriteLine(s_dalVolunteer.Read(id));
                break;
            case "call":
                Console.WriteLine(s_dalCall.Read(id));
                break;
            case "assignment":
                Console.WriteLine(s_dalAssignment.Read(id));
                break;
            default:
                throw ($"ther is no type of config like {typeOf}");
                break
        }

    }
    private static ConfigMenuFunction()
    {
        MainMenu option;
        do
        {
            console.WriteLine("\n SubConfigMenu are on");
            foreach (var option in Enum.GetValues(typeof(MainMenu)))
            {
                Console.WriteLine($"\n Enter {(int)option} to {option.ToString().Replace('_', ' ').ToLower()}");
            }
            option = (MainMenu)GetIntFromUser("\n enter your choice: ");
            Console.Clear();
            switch (option)
            {
                case SubConfigMenu.exit:
                    Console.WriteLine("\n have a good day!");
                    break;
                case SubConfigMenu.minute:
                    changeTimeFunction("minute");
                    break;
                case SubConfigMenu.hour:
                    changeTimeFunction("hour");
                    break;
                case SubConfigMenu.day:
                    changeTimeFunction("day");
                    break;
                case SubConfigMenu.month:
                    changeTimeFunction("month");
                    break;
                case SubConfigMenu.year:
                    changeTimeFunction("year");
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
                    throw ("\n ther is no option like this");
                    break;
            }
        }
        while (option != MainMenu.exit);
    }
    private static ResetDataFunction()
    {
        s_dalConfig.Reset();
        s_dalAssignment.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalVolunteer.DeleteAll();
    }
    private static CreateFunction(typeOf)
    {
        if (typeOf == "volunteers")
        {
            Console.WriteLine("\n Enter ID");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("\n Enter valid input");
            Console.WriteLine("\n Enter first name");
            string firstName = Console.ReadLine();
            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            Console.WriteLine("\n Enter phoneNumber");
            long phoneNumber = Console.ReadLine();
            Console.WriteLine("\n Enter email address");
            string email = Console.ReadLine();
            Console.WriteLine("\n Enter password");
            string password = Console.ReadLine();
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            Console.WriteLine("\n Enter Latitude");
            double Latitude = Console.ReadLine();
            Console.WriteLine("\n Enter Longitude");
            double Longitude = Console.ReadLine();
            Volunteer volunteer = new Volunteer(id, firstName, lastName, phoneNumber, email, password, address, Latitude, Longitude);
            s_dalVolunteer.Create(volunteer);
        }
        else if (typeOf == "calls")
        {
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            Console.WriteLine("\n Enter Latitude");
            double Latitude = Console.ReadLine();
            Console.WriteLine("\n Enter Longitude");
            double Longitude = Console.ReadLine();
            Call call = new Call(description, address, Latitude, Longitude);
            s_dalCall.Create(call)
        }
        else if (typeOf == "assignments")
        {

        }
        else throw ("ERROR");
    }
    private static ReadFunction(typeOf)
    {
        Console.WriteLine("enter ID for display");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input");
        if (typeOf == "volunteer")
            Console.WriteLine(s_dalVolunteer.Read(id));
        else if (typeOf == "calls")
            Console.WriteLine(s_dalCall.Read(id));
        else if (typeOf == "assingments")
            Console.WriteLine(s_dalAssignment.Read(id));
        else throw ("ERROR");
    }
    private static ReadAllFunction(typeOf)
    {
        if (typeOf == "volunteer")
            var list = s_dalVolunteer.ReadAll();
        else if (typeOf == "calls")
            var list = s_dalCall.ReadAll();
        else if (typeOf == "assingments")
            var list = s_dalAssignment.ReadAll();
        else if (list.count == 0)
            Console.WriteLine($"\n there are no {typeOf} for display");
        else throw ("ERROR");
        foreach (var item in list)
            Console.WriteLine($"\n {item}");
    }
    private static UpdateFunction(typeOf)
    {
        if (typeOf == "volunteers")
        {
            Console.WriteLine("\n Enter ID");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("\n Enter valid input");
            Console.WriteLine("\n Enter first name");
            string firstName = Console.ReadLine();
            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            Console.WriteLine("\n Enter last name");
            string lastName = Console.ReadLine();
            Console.WriteLine("\n Enter phoneNumber");
            long phoneNumber = Console.ReadLine();
            Console.WriteLine("\n Enter email address");
            string email = Console.ReadLine();
            Console.WriteLine("\n Enter password");
            string password = Console.ReadLine();
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            Console.WriteLine("\n Enter Latitude");
            double Latitude = Console.ReadLine();
            Console.WriteLine("\n Enter Longitude");
            double Longitude = Console.ReadLine();
            Volunteer volunteer = new Volunteer(id, firstName, lastName, phoneNumber, email, password, address, Latitude, Longitude);
            s_dalVolunteer.Update(volunteer);
        }
        else if (typeOf == "calls")
        {
            Console.WriteLine("\n Enter description");
            string description = Console.ReadLine();
            Console.WriteLine("\n Enter address");
            string address = Console.ReadLine();
            Console.WriteLine("\n Enter Latitude");
            double Latitude = Console.ReadLine();
            Console.WriteLine("\n Enter Longitude");
            double Longitude = Console.ReadLine();
            Call call = new Call(description, address, Latitude, Longitude);
            s_dalCall.Update(call)
        }
        else if (typeOf == "assignments")
        {
        }
        else throw ("ERROR");
    }
    private static DeleteFunction(typeOf)
    {
        Console.WriteLine("\n Enter ID");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input");
        if (typeOf == "volunteers")
            s_dalVolunteer.Delete(id);
        else if (typeOf == "calls")
            s_dalCall.Delete(id);
        else if (typeOf == "assignments")
            s_dalAssignment.Delete(id);
        else throw ("ERROR");
    }
    private static DeleteAllFunction(typeOf)
    {
        if (typeOf == "volunteers")
            s_dalVolunteer.DeleteAll();
        else if (typeOf == "calls")
            s_dalCall.DeleteAll();
        else if (typeOf == "assignments")
            s_dalAssignment.DeleteAll();
        else throw ("ERROR");
    }

    private static changeTimeFunction(typeOf)
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
        else throw ("\n ERROR");
    }
    private static DisplayClockSystemFunction()
    {
        Console.WriteLine($"the system clock is: {s_dalConfig.Clock}");
    }
    private static RedefineFunction()
    {
        Console.WriteLine("\n enter type of config (volunteer, call, assignment)");
        string typeOf = Console.ReadLine();
        Console.WriteLine("\n enter ID");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
            Console.WriteLine("\n Enter valid input");
        var result;
        switch (typeOf)
        {
            case "volunteer":

                Console.WriteLine("\n enter type to change ( first name, last name, address, phone number, email)");
                string option = Console.ReadLine();
                Console.WriteLine("\n Enter ID");
                int id;
                var volunteerToChange = s_dalVolunteer.Read(id);
                while (!int.TryParse(Console.ReadLine(), out id))
                    Console.WriteLine("\n Enter valid input");
                switch (option)
                {
                    case "first name":
                        Console.Write("\n Enter First Name: ");
                        result = Console.ReadLine();
                        volunteerToChange.FirstName = result;
                        break;
                    case "last name":
                        Console.Write("\n Enter Last Name: ");
                        result = Console.ReadLine();
                        volunteerToChange.LastName = result;
                        break;
                    case "address":
                        Console.Write("\n Enter Address: ");
                        result = Console.ReadLine();
                        volunteerToChange.Address = result;
                        break;
                    case "phone number":
                        Console.Write("\n Enter Phone Number: ");
                        result = Console.ReadLine();
                        volunteerToChange.PhoneNumber = result;
                        break;
                    case "email":
                        Console.Write("\n Enter Email: ");
                        result = Console.ReadLine();
                        volunteerToChange.Email = result;
                        break;
                    default:
                        throw("\n Invalid input");
                        break;
                }
                s_dalVolunteer.Update(volunteerToChange);
            case "call":
                Console.WriteLine("\n enter type to change (description, address)");
                string option = Console.ReadLine();
                Console.WriteLine("\n Enter ID");
                int id;
                var callToChange = s_dalCall.Read(id);
                while (!int.TryParse(Console.ReadLine(), out id))
                    Console.WriteLine("\n Enter valid input");
                switch (option)
                {
                    case "description":
                        Console.Write("\n Enter First Name: ");
                        result = Console.ReadLine();
                        toChangeToChange.description = result;
                        break;
                    case "description":
                        Console.Write("\n Enter Last Name: ");
                        result = Console.ReadLine();
                        toChangeToChange.Address = result;
                        break;
                    default:
                        throw ("\n Invalid input");
                        break;
                }
                s_dalCall.Update(callToChange);
                break;
            case "assignment":
                break;
            default:
                throw ($"ther is no type of config like {typeOf}");
                break;
        }
    }
    private static DisplayClockConfigFunction()
    {
        Console.WriteLine(s_dalConfig.Clock());
    }
    private static ResetFunction()
    {
        s_dalConfig.reset();
    }