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
public interface IAdmin
{
    DateTime GetSystemClock();
    void AdvanceSystemClock(TimeUnit unit);
    TimeSpan GetRiskTimeSpan();
    void SetRiskTimeSpan(TimeSpan timeSpan);
    void ResetDatabase();
    void InitializeDatabase();
}
