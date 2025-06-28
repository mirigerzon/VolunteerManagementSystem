//using DalApi;
//using BlApi;
//using BO;
//using Helpers;

//namespace BlImplementation;

//internal class AdminImplementation : IAdmin
//{
//    private readonly IDal _dal = DalApi.Factory.Get;
//    // Retrieves the current system clock time from the database
//    public DateTime GetSystemClock()
//    {
//        try
//        {
//            return _dal.Config.Clock;
//        }
//        catch (DO.DalXMLFileLoadCreateException ex)
//        {
//            throw new BlXMLFileLoadCreateException("Failed to load system clock from the database.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new BlInvalidException("Unexpected error while retrieving system clock.", ex);
//        }
//    }
//    // Advances the system clock by a specified time unit (minute, hour, or day)
//    public void AdvanceSystemClock(TimeUnit unit)
//    {
//        try
//        {
//            Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
//            switch (unit)
//            {
//                case TimeUnit.Minute:
//                    AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
//                    break;
//                case TimeUnit.Hour:
//                    AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
//                    break;
//                case TimeUnit.Day:
//                    AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
//                    break;
//                case TimeUnit.Year:
//                    AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
//                    break;
//                default:
//                    throw new BlInvalidException($"Unsupported time unit: {unit}");
//            }
//        }
//        catch (Exception ex)
//        {
//            throw new BlInvalidException("Failed to advance system clock.", ex);
//        }
//    }
//    // Retrieves the current configured risk time span from the database
//    public TimeSpan GetRiskTimeSpan()
//    {
//        try
//        {
//            return AdminManager.RiskRange;
//        }
//        catch (DO.DalXMLFileLoadCreateException ex)
//        {
//            throw new BlXMLFileLoadCreateException("Failed to load risk time span from the database.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new BlInvalidException("Unexpected error while retrieving risk time span.", ex);
//        }
//    }
//    // Sets a new value for the risk time span in the database
//    public void SetRiskTimeSpan(TimeSpan timeSpan)
//    {
//        try
//        {
//            Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
//            AdminManager.RiskRange = timeSpan;
//        }
//        catch (DO.DalXMLFileLoadCreateException ex)
//        {
//            throw new BlXMLFileLoadCreateException("Failed to update risk time span in the database.", ex);
//        }
//        catch (Exception ex)
//        {
//            throw new BlInvalidException("Unexpected error while setting risk time span.", ex);
//        }
//    }
//    // Resets the database and updates the system clock to the current time
//    public void ResetDatabase() //stage 4
//    {
//        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
//        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
//        AdminManager.ResetDB(); //stage 7
//    }
//    // Re-initializes the database with test data and resets the system clock
//    public void InitializeDatabase() //stage 4
//    {
//        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
//        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
//        AdminManager.InitializeDB(); //stage 7
//    }
//    public void AddClockObserver(Action clockObserver) =>
//    AdminManager.ClockUpdatedObservers += clockObserver;
//    public void RemoveClockObserver(Action clockObserver) =>
//    AdminManager.ClockUpdatedObservers -= clockObserver;
//    public void AddConfigObserver(Action configObserver) =>
//    AdminManager.ConfigUpdatedObservers += configObserver;
//    public void RemoveConfigObserver(Action configObserver) =>
//    AdminManager.ConfigUpdatedObservers -= configObserver;
//    public void StartSimulator(int interval) //stage 7
//    {
//        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
//        AdminManager.Start(interval); //stage 7
//    }
//    public void StopSimulator()
//=> AdminManager.Stop(); //stage 7
//    public void AddObserver(Action listObserver) =>
//    AdminManager.Observers.AddListObserver(listObserver); //stage 5
//    public void AddObserver(int id, Action observer) =>
//    AdminManager.Observers.AddObserver(id, observer); //stage 5
//    public void RemoveObserver(Action listObserver) =>
//    AdminManager.Observers.RemoveListObserver(listObserver); //stage 5
//    public void RemoveObserver(int id, Action observer) =>
//    AdminManager.Observers.RemoveObserver(id, observer); //stage 5
//}
using DalApi;
using BlApi;
using BO;
using Helpers;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly IDal _dal = DalApi.Factory.Get;

    // Retrieves the current system clock time from the database
    public DateTime GetSystemClock()
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7 - lock added
                return _dal.Config.Clock; //stage 4
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BlXMLFileLoadCreateException("Failed to load system clock from the database.", ex);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Unexpected error while retrieving system clock.", ex);
        }
    }

    // Advances the system clock by a specified time unit (minute, hour, or day)
    public void AdvanceSystemClock(TimeUnit unit)
    {
        try
        {
            Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
            switch (unit)
            {
                case TimeUnit.Minute:
                    AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                    break;
                case TimeUnit.Hour:
                    AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                    break;
                case TimeUnit.Day:
                    AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                    break;
                case TimeUnit.Year:
                    AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                    break;
                default:
                    throw new BlInvalidException($"Unsupported time unit: {unit}");
            }
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to advance system clock.", ex);
        }
    }

    // Retrieves the current configured risk time span from the database
    public TimeSpan GetRiskTimeSpan()
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7 - lock added
                return AdminManager.RiskRange;
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BlXMLFileLoadCreateException("Failed to load risk time span from the database.", ex);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Unexpected error while retrieving risk time span.", ex);
        }
    }

    // Sets a new value for the risk time span in the database
    public void SetRiskTimeSpan(TimeSpan timeSpan)
    {
        try
        {
            Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
            lock (AdminManager.BlMutex) //stage 7 - lock added
                AdminManager.RiskRange = timeSpan;
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BlXMLFileLoadCreateException("Failed to update risk time span in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Unexpected error while setting risk time span.", ex);
        }
    }

    // Resets the database and updates the system clock to the current time
    public void ResetDatabase() //stage 4
    {
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        lock (AdminManager.BlMutex) //stage 7 - lock added
            AdminManager.ResetDB(); //stage 7
    }

    // Re-initializes the database with test data and resets the system clock
    public void InitializeDatabase() //stage 4
    {
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        lock (AdminManager.BlMutex) //stage 7 - lock added
            AdminManager.InitializeDB(); //stage 7
    }

    public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;

    public void StartSimulator(int interval) //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.Start(interval); //stage 7
    }

    public void StopSimulator() =>
        AdminManager.Stop(); //stage 7

    public void AddObserver(Action listObserver) =>
        AdminManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        AdminManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        AdminManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        AdminManager.Observers.RemoveObserver(id, observer); //stage 5
}