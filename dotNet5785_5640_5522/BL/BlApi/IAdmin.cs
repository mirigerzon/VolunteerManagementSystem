using System;
using BO;

namespace BlApi;

/// <summary>
/// Interface for system administration tasks.
/// Includes:
/// - GetSystemClock: returns the current system clock value.
/// - AdvanceSystemClock: advances the system clock by a specified time unit.
/// - GetRiskTimeSpan: retrieves the current risk time span configuration.
/// - SetRiskTimeSpan: updates the risk time span configuration.
/// - ResetDatabase: resets configuration and clears all entities.
/// - InitializeDatabase: resets and then populates the database with initial data.
/// </summary>
public interface IAdmin : IObservable
{
    DateTime GetSystemClock();
    void AdvanceSystemClock(TimeUnit unit);
    TimeSpan GetRiskTimeSpan();
    void SetRiskTimeSpan(TimeSpan timeSpan);
    void ResetDatabase();
    void InitializeDatabase();
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7
}
