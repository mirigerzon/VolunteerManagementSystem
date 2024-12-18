namespace Dal;
using DalApi;
internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
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