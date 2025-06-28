namespace Dal;
using DalApi;
using System.Runtime.CompilerServices;

internal class ConfigImplementation : IConfig
{
    // Gets or sets the global clock value from the configuration.
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
    // Retrieves and increments the next available Assignment ID from the configuration.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int GetNextAssignmentId()
    {
        return Config.NextAssignmentId;
    }
    // Retrieves and increments the next available Call ID from the configuration.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int GetNextCallId()
    {
        return Config.NextCallId;
    }
    // Resets the configuration values to their initial states.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void reset()
    {
        Config.Reset();
    }
}