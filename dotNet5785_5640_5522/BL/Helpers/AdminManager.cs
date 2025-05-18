using BlImplementation;
using BO;
using DalApi;
using System.Threading;
namespace Helpers;

/// &lt;summary&gt;
/// Internal BL manager for all Application's Clock logic policies
/// &lt;/summary&gt;
internal static class AdminManager //stage 4
{
    internal static ObserverManager Observers = new(); //stage 5

    #region Stage 4
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4
    /// &lt;summary&gt;
    /// Property for providing/setting current configuration variable value for any BL class that may need it
    /// &lt;/summary&gt;
    internal static TimeSpan RiskRange
    {
        get => s_dal.Config.RiskRange;
        set
        {
            s_dal.Config.RiskRange = value;
            ConfigUpdatedObservers?.Invoke(); // stage 5
        }
    }

    /// &lt;summary&gt;
    /// Property for providing current application's clock value for any BL class that may need it
    /// &lt;/summary&gt;
    internal static DateTime Now
    {
        get => s_dal.Config.Clock;
    } //stage 4

    /// &lt;summary&gt;
    /// Method to perform application's clock from any BL class as may be required
    /// &lt;/summary&gt;
    /// &lt;param name="newClock"&gt;updated clock value&lt;/param&gt;
    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {
        // new Thread(() =&gt; { // stage 7 - not sure - still under investigation - see stage 7 instructions after it will be released        
        updateClock(newClock);//stage 4-6
        // }).Start(); // stage 7 as above
    }

    private static void updateClock(DateTime newClock) // prepared for stage 7 as DRY to eliminate needless repetition
    {
        var oldClock = s_dal.Config.Clock; //stage 4
        s_dal.Config.Clock = newClock; //stage 4

        //TO_DO:
        //Add calls here to any logic method that should be called periodically,
        //after each clock update
        //for example, Periodic students' updates:
        //Go through all students to update properties that are affected by the clock update
        //(students becomes not active after 5 years etc.)

        VolunteerManager.PeriodicVolunteersUpdates(oldClock, newClock); //stage 4
        //etc ...

        //Calling all the observers of clock update
        ClockUpdatedObservers?.Invoke(); //prepared for stage 5
    }
    #endregion Stage 4

    #region Stage 5
    internal static event Action? ConfigUpdatedObservers; //prepared for stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers
    internal static void InitializeDB()
    {
        //lock (BlMutex) //stage 7
        {
            DalTest.Initialization.Do();
            AdminManager.UpdateClock(AdminManager.Now); // stage 5 - needed for update the PL
            AdminManager.RiskRange = AdminManager.RiskRange; // stage 5 - needed for update the PL
            ClockUpdatedObservers?.Invoke(); //prepared for stage 5
        }
    }
    internal static void ResetDB()
    {
        //lock (BlMutex) //stage 7
        {
            s_dal.ResetDB();
            AdminManager.UpdateClock(AdminManager.Now); //stage 5 - needed for update PL
            AdminManager.RiskRange = AdminManager.RiskRange; //stage 5 - needed for update PL
            ClockUpdatedObservers?.Invoke(); //prepared for stage 5

        }
    }
    #endregion Stage 5

    #region Stage 7 base
    internal static readonly object blMutex = new();
    private static Thread? s_thread;
    private static int s_interval { get; set; } = 1; //in minutes by second    
    private static volatile bool s_stop = false;
    private static readonly object mutex = new();

    internal static void Start(int interval)
    {
        lock (mutex)
            if (s_thread == null)
            {
                s_interval = interval;
                s_stop = false;
                s_thread = new Thread(clockRunner);
                s_thread.Start();
            }
    }

    internal static void Stop()
    {
        lock (mutex)
            if (s_thread != null)
            {
                s_stop = true;
                s_thread?.Interrupt();
                s_thread = null;
            }
    }

    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));

            #region Stage 7
            //TO_DO:
            //Add calls here to any logic simulation that was required in stage 7
            //for example: course registration simulation
            //StudentManager.SimulateCourseRegistrationAndGrade(); //stage 7

            //etc...
            #endregion Stage 7

            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException) { }
        }
    }
    #endregion Stage 7 base
}