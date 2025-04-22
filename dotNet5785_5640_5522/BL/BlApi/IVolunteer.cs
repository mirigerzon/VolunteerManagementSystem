using BO;
using System;
using System.Collections.Generic;

namespace VolunteerApp
{
    public interface IVolunteer
    {
        // Login using username and password - returns the user's role in the system
        UserRole Login(string username, string password);

        // Returns a list of volunteers filtered by activity status (active/inactive) and sorted by the selected field
        IEnumerable<VolunteerInList> GetVolunteers(bool? isActive, VolunteerSortField? sortBy);

        // Returns the volunteer's details by ID, including current calls handled
        Volunteer GetVolunteer(string id);

        // Updates the details of an existing volunteer
        void UpdateVolunteer(string updaterId, Volunteer updatedVolunteer);

        // Deletes a volunteer by ID, only if they have not handled any calls or are not currently handling any
        void DeleteVolunteer(string id);

        // Adds a new volunteer
        void AddVolunteer(Volunteer newVolunteer);
    }
    public enum UserRole
    {
        Admin,
        Volunteer,
        Coordinator
    }
    public enum VolunteerSortField
    {
        Id,
        Name,
        Status,
        City
    }
}
