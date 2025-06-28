namespace Dal;
using DalApi;
using System.Runtime.CompilerServices;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.RiskRange + TimeSpan.FromHours(1);
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.RiskRange = value;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int GetNextAssignmentId()
    {
        return Config.NextAssignmentId;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int GetNextCallId()
    {
        return Config.NextCallId;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void reset()
    {
        Config.Reset();
    }
}