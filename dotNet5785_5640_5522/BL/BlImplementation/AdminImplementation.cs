using DalApi;
using BlApi;
using BO;
using Helpers;
using DalTest;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DateTime GetSystemClock()
    {
        return _dal.Config.Clock;
    }
    public void AdvanceSystemClock(TimeUnit unit)
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
                throw new ArgumentOutOfRangeException(nameof(unit), "Unsupported time unit");
        }
    }
    public TimeSpan GetRiskTimeSpan()
    {
        return _dal.Config.RiskRange;
    }            
    public void SetRiskTimeSpan(TimeSpan timeSpan)
    {
        _dal.Config.RiskRange = timeSpan;
    }  
    public void ResetDatabase()
    {
        ClockManager.UpdateClock(ClockManager.Now);
        _dal.ResetDB();
    }
    public void InitializeDatabase()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}