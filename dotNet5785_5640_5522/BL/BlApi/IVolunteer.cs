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
public interface IVolunteer
{
    UserRole Login(string username, string password);
    IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive = null, VolunteerSortField? sortBy = null);
    BO.Volunteer GetVolunteer(int id);
    void UpdateVolunteer(int id, BO.Volunteer volunteer);
    void AddVolunteer(BO.Volunteer volunteer);
    void RemoveVolunteer(int id);
}
