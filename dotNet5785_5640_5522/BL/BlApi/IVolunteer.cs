using System;
using BO;
using System.Collections.Generic;

namespace BlApi;

/// <summary>
/// Interface for managing volunteers.
/// Includes:
/// - Login: authenticate volunteer.
/// - GetVolunteers: retrieve list of volunteers with optional filters.
/// - GetVolunteer: get full details of a specific volunteer.
/// - UpdateVolunteer: update volunteer information.
/// - AddVolunteer: add a new volunteer.
/// - RemoveVolunteer: deactivate volunteer.
/// </summary>
public interface IVolunteer : IObservable
{
    UserRole Login(int id, string password);
    IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, VolunteerSortField? sortBy = null);
    BO.Volunteer Read(int id);
    void Update(int id, Volunteer volunteer);
    Task Create(Volunteer volunteer);
    void Delete(int id);
}