using DalApi;
using BlApi;
using BO;
using Helpers;
using DalTest;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly IDal _dal = DalApi.Factory.Get;
    public DateTime GetSystemClock()
    {
        try
        {
            return _dal.Config.Clock;
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
    public void AdvanceSystemClock(TimeUnit unit)
    {
        try
        {
            switch (unit)
            {
                case TimeUnit.Minute:
                    ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
                    break;
                case TimeUnit.Hour:
                    ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                    break;
                case TimeUnit.Day:
                    ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
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
    public TimeSpan GetRiskTimeSpan()
    {
        try
        {
            return _dal.Config.RiskRange;
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
    public void SetRiskTimeSpan(TimeSpan timeSpan)
    {
        try
        {
            _dal.Config.RiskRange = timeSpan;
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
    public void ResetDatabase()
    {
        try
        {
            ClockManager.UpdateClock(ClockManager.Now);
            _dal.ResetDB();
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BlXMLFileLoadCreateException("Failed to reset the database.", ex);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Unexpected error while resetting the database.", ex);
        }
    }
    public void InitializeDatabase()
    {
        try
        {
            _dal.ResetDB();    
            ClockManager.UpdateClock(ClockManager.Now);
            DalTest.Initialization.Do();  
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BlXMLFileLoadCreateException("Failed to initialize the database.", ex);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Unexpected error while initializing the database.", ex);
        }
    }

}
