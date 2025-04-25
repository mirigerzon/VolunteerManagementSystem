namespace Dal;
using DalApi;
internal class ConfigImplementation : IConfig
{
    // Gets or sets the global clock value from the configuration.
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange { get; set; }

    // Retrieves and increments the next available Assignment ID from the configuration.
    public int GetNextAssignmentId()
    {
        return Config.NextAssignmentId;
    }
    // Retrieves and increments the next available Call ID from the configuration.
    public int GetNextCallId()
    {
        return Config.NextCallId;
    }
    // Resets the configuration values to their initial states.
    public void reset()
    {
        Config.Reset();
    }
}