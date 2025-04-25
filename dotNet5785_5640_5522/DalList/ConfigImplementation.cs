namespace Dal;
using DalApi;
internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange
    {
        get => Config.RiskRange + TimeSpan.FromHours(1);
        set => Config.RiskRange = value;
    }

    public int GetNextAssignmentId()
    {
        return Config.NextAssignmentId;
    }

    public int GetNextCallId()
    {
        return Config.NextCallId;
    }

    public void reset()
    {
        Config.Reset();
    }
}