namespace Dal;
using DalApi;
public class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public void reset()
    {
        Config.Reset();
    }
}
