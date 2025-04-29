﻿//using BlImplementation;
//using BO;
//namespace Helpers;

///// <summary>
///// 
///// </summary>
//internal static class ClockManager //stage 4
//{
//    #region Stage 4
//    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
//    internal static DateTime Now { get => _dal.Config.Clock; } //stage 4

//    //private static readonly int STAGE = 7; //set to your current stage //XXX ???

//    internal static void UpdateClock(DateTime newClock) //stage 4-7
//    {
//        //if (STAGE < 7) //XXX ???
//            //stage 4-6
//            updateClock(newClock);
//        //else
//        //    //stage 7
//        //    new Thread(() => updateClock(newClock)).Start();
//    }

//    private static void updateClock(DateTime newClock) // prepared for stage 7 as DRY to eliminate needless repetition
//    {
//        var oldClock = _dal.Config.Clock; //stage 4
//        _dal.Config.Clock = newClock; //stage 4

//        //TO_DO:
//        //Add calls here to any logic method that should be called periodically,
//        //after each clock update
//        //for example, Periodic students' updates:
//        //Go through all students to update properties that are affected by the clock update
//        //(students becomes not active after 5 years etc.)

//        StudentManager.PeriodicStudentsUpdates(oldClock, newClock); //stage 4
//        //etc ...

//        //Calling all the observers of clock update
//        ClockUpdatedObservers?.Invoke(); //prepared for stage 5
//    }
//    #endregion Stage 4

//    //#region Stage 5

//    //internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers

//    //#endregion Stage 5


//    //#region Stage 7 base
//    //internal static readonly object blMutex = new();
//    //private static Thread? s_thread;
//    //private static int s_interval { get; set; } = 1; //in minutes by second    
//    //private static volatile bool s_stop = false;
//    //private static object mutex = new();

//    //internal static void Start(int interval)
//    //{
//    //    lock (mutex)
//    //        if (s_thread == null)
//    //        {
//    //            s_interval = interval;
//    //            s_stop = false;
//    //            s_thread = new Thread(clockRunner);
//    //            s_thread.Start();
//    //        }
//    //}

//    //internal static void Stop()
//    //{
//    //    lock (mutex)
//    //        if (s_thread != null)
//    //        {
//    //            s_stop = true;
//    //            s_thread?.Interrupt();
//    //            s_thread = null;
//    //        }
//    //}

//    //private static void clockRunner()
//    //{
//    //    while (!s_stop)
//    //    {
//    //        UpdateClock(Now.AddMinutes(s_interval));

//    //        #region Stage 7
//    //        //TO_DO:
//    //        //Add calls here to any logic simulation that was required in stage 7
//    //        //for example: course registration simulation
//    //        StudentManager.SimulateCourseRegistrationAndGrade(); //stage 7

//    //        //etc...
//    //        #endregion Stage 7

//    //        try
//    //        {
//    //            Thread.Sleep(1000); // 1 second
//    //        }
//    //        catch (ThreadInterruptedException) { }
//    //    }
//    //}
//    //#endregion Stage 7 base
//}
using BlImplementation;
using BO;
namespace Helpers;

/// <summary>
/// 
/// </summary>
internal static class ClockManager //stage 4
{
    #region Stage 4
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
    internal static DateTime Now { get => _dal.Config.Clock; } //stage 4

    //private static readonly int STAGE = 7; //set to your current stage //XXX ???

    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {
        //if (STAGE < 7) //XXX ???
        //stage 4-6
        updateClock(newClock);
        //else
        //    //stage 7
        //    new Thread(() => updateClock(newClock)).Start();
    }

    private static void updateClock(DateTime newClock) // prepared for stage 7 as DRY to eliminate needless repetition
    {
        var oldClock = _dal.Config.Clock; //stage 4
        _dal.Config.Clock = newClock; //stage 4

        //TO_DO:
        //Add calls here to any logic method that should be called periodically,
        //after each clock update
        //for example, Periodic students' updates:
        //Go through all students to update properties that are affected by the clock update
        //(students becomes not active after 5 years etc.)

        CallManager.PeriodicCallsUpdates(oldClock, newClock); //stage 4
        VolunteerManager.PeriodicCallsUpdates(oldClock, newClock); //stage 4

        //Calling all the observers of clock update
        /*ClockUpdatedObservers?.Invoke();*/ //prepared for stage 5
    }
    #endregion Stage 4


    //#region Stage 5

    //internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers

    //#endregion Stage 5


    //#region Stage 7 base
    //internal static readonly object blMutex = new();
    //private static Thread? s_thread;
    //private static int s_interval { get; set; } = 1; //in minutes by second    
    //private static volatile bool s_stop = false;
    //private static object mutex = new();

    //internal static void Start(int interval)
    //{
    //    lock (mutex)
    //        if (s_thread == null)
    //        {
    //            s_interval = interval;
    //            s_stop = false;
    //            s_thread = new Thread(clockRunner);
    //            s_thread.Start();
    //        }
    //}

    //internal static void Stop()
    //{
    //    lock (mutex)
    //        if (s_thread != null)
    //        {
    //            s_stop = true;
    //            s_thread?.Interrupt();
    //            s_thread = null;
    //        }
    //}

    //private static void clockRunner()
    //{
    //    while (!s_stop)
    //    {
    //        UpdateClock(Now.AddMinutes(s_interval));

    //        #region Stage 7
    //        //TO_DO:
    //        //Add calls here to any logic simulation that was required in stage 7
    //        //for example: course registration simulation
    //        StudentManager.SimulateCourseRegistrationAndGrade(); //stage 7

    //        //etc...
    //        #endregion Stage 7

    //        try
    //        {
    //            Thread.Sleep(1000); // 1 second
    //        }
    //        catch (ThreadInterruptedException) { }
    //    }
    //}
    //#endregion Stage 7 base
}