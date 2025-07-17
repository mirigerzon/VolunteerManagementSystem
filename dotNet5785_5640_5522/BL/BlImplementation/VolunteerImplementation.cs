using BO;
using DalApi;
using DO;
using Helpers;
namespace BlImplementation;
public class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly IDal _dal = DalApi.Factory.Get;
    // Logs in the volunteer by verifying their credentials and returns their user role
    public UserRole Login(int id, string password)
    {
        try
        {
            lock (Helpers.AdminManager.BlMutex) // STAGE 7 - LOCK
            {
                var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == id && v.Password == password && (v.IsActive == true || v.Role == Enums.RoleEnum.Admin));
                if (volunteer == null)
                    throw new BlDoesNotExistException("Invalid credentials");
                UserRole role = (UserRole)volunteer.Role;
                return role;
            }
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Login failed", ex);
        }
    }
    // Retrieves all volunteers, with optional filtering and sorting by specified fields
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, VolunteerSortField? sortBy = null)
    {
        List<DO.Volunteer> volunteers;
        List<DO.Assignment> assignments;

        lock (Helpers.AdminManager.BlMutex)
        {
            volunteers = _dal.Volunteer.ReadAll().ToList();
            assignments = _dal.Assignment.ReadAll().ToList();
        }

        if (isActive.HasValue)
            volunteers = volunteers.Where(v => v.IsActive == isActive.Value).ToList();

        try
        {
            if (sortBy.HasValue)
            {
                switch (sortBy.Value)
                {
                    case VolunteerSortField.FullName:
                        volunteers = volunteers.OrderBy(v => v.FullName).ToList();
                        break;
                    case VolunteerSortField.Id:
                        volunteers = volunteers.OrderBy(v => v.Id).ToList();
                        break;
                    case VolunteerSortField.IsActive:
                        volunteers = volunteers.OrderBy(v => v.IsActive).ToList();
                        break;
                    default:
                        throw new BlInvalidException("Sorting failed - Cannot sort by " + sortBy.Value);
                }
            }
            else
            {
                volunteers = volunteers.OrderBy(v => v.Id).ToList();
            }
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Sorting failed.", ex);
        }

        return volunteers.Select(v =>
        {
            var openAssignment = assignments
                .FirstOrDefault(a => a.VolunteerId == v.Id && a.EndTime == null);

            return new BO.VolunteerInList
            {
                Id = v.Id,
                FullName = v.FullName,
                IsActive = v.IsActive,
                TotalHandledCalls = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.Treated),
                TotalCanceledCalls = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.SelfCancelled) + Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.ManagerCancelled),
                ExpiredCallsCount = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.Expired),
                CallType = Helpers.VolunteerManager.CallTypeIfExist(v.Id),
                CurrentCallId = openAssignment?.CallId
            };
        }).ToList();
    }

    // Retrieves a specific volunteer by their ID
    public BO.Volunteer Read(int id)
    {
        try
        {
            DO.Volunteer volunteer;
            lock (Helpers.AdminManager.BlMutex) // STAGE 7 - LOCK
            {
                volunteer = _dal.Volunteer.Read(id);
            }

            if (volunteer == null)
                throw new BlDoesNotExistException("Volunteer not found");
            DO.Assignment? openAssignment;
            lock (Helpers.AdminManager.BlMutex)
            {
                openAssignment = _dal.Assignment.ReadAll()
                    .FirstOrDefault(a => a.VolunteerId == id && a.EndTime == null);
            }

            BO.CallInProgress? currentCall = null;
            if (openAssignment != null)
            {
                currentCall = new BO.CallInProgress
                {
                    Id = openAssignment.Id,
                    CallId = openAssignment.CallId,
                    StartTime = openAssignment.ArrivalTime!.Value,
                    Status = CallInProgressStatus.InProgress
                };
            }

            return new BO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                Password = volunteer.Password,
                Address = volunteer.Address,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                Role = (UserRole)volunteer.Role,
                IsActive = volunteer.IsActive,
                MaxDistance = volunteer.MaxOfDistance,
                TypeOfDistance = (TypeOfDistance)volunteer.TypeOfDistance,
                CurrentCall = currentCall,
            };
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Error retrieving volunteer.", ex);
        }
    }
    // Updates an existing volunteer's information
    public void Update(int id, BO.Volunteer volunteer)
    {
        try
        {
            Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
            DO.Volunteer dalVolunteer;

            lock (Helpers.AdminManager.BlMutex)
            {
                dalVolunteer = _dal.Volunteer.Read(id);
            }
            if (dalVolunteer == null)
                throw new BlDoesNotExistException("Volunteer not found");
            if (dalVolunteer.Id == volunteer.Id || dalVolunteer.Role == DO.Enums.RoleEnum.Admin)
            {
                if ((UserRole)dalVolunteer.Role != volunteer.Role && (UserRole)dalVolunteer.Role != UserRole.Admin)
                    throw new BlInvalidException("Volunteer cannot change roles!");
                string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
                if (checkValues != "true")
                    throw new BlInvalidException(checkValues + " - this field is not valid");
                DO.Volunteer volunteerWithoutCoords = new DO.Volunteer
                {
                    Id = volunteer.Id,
                    FullName = volunteer.FullName,
                    PhoneNumber = volunteer.PhoneNumber,
                    Email = volunteer.Email,
                    Password = volunteer.Password,
                    Address = volunteer.Address,
                    Latitude = null,
                    Longitude = null,
                    Role = (DO.Enums.RoleEnum)volunteer.Role,
                    IsActive = volunteer.IsActive,
                    MaxOfDistance = volunteer.MaxDistance,
                    TypeOfDistance = (DO.Enums.TypeOfDistanceEnum)volunteer.TypeOfDistance
                };
                lock (Helpers.AdminManager.BlMutex)
                {
                    _dal.Volunteer.Update(volunteerWithoutCoords);
                }
                VolunteerManager.Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyItemUpdated(id);
                _ = Helpers.VolunteerManager.UpdateCoordinatesForVolunteerAddressAsync(volunteerWithoutCoords);
            }
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Error updating volunteer.", ex);
        }
    }
    // Deletes a volunteer by their ID
    public void Delete(int id)
    {
        try
        {
            Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7

            DO.Volunteer volunteer;
            lock (Helpers.AdminManager.BlMutex) // STAGE 7 - LOCK
            {
                volunteer = _dal.Volunteer.Read(id);
                if (volunteer == null)
                    throw new BlDoesNotExistException("Volunteer not found");

                _dal.Volunteer.Delete(volunteer.Id);
            }

            VolunteerManager.Observers.NotifyListUpdated();        // STAGE 5 - ADDED
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Error deactivating volunteer.", ex);
        }
    }
    // Creates a new volunteer and adds them to the database
    public async Task Create(BO.Volunteer volunteer)
    {
        try
        {
            Helpers.AdminManager.ThrowOnSimulatorIsRunning(); // STAGE 7
            string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
            if (checkValues != "true")
                throw new BlInvalidException($"{checkValues} - this field is not valid");
            var newVolunteer = await Helpers.VolunteerManager.ConvertBoToDoAsync(volunteer);
            lock (Helpers.AdminManager.BlMutex) // STAGE 7 - LOCK
            {
                _dal.Volunteer.Create(newVolunteer);
            }
            VolunteerManager.Observers.NotifyListUpdated(); // STAGE 5
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BlAlreadyExistsException("Volunteer already exists.", ex);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Error adding volunteer.", ex);
        }
    }
    public IEnumerable<Assignment> GetAssignmentHistory(int volunteerId)
    {
        try
        {
            List<Assignment> assignments;
            lock (Helpers.AdminManager.BlMutex) // STAGE 7 - LOCK
            {
                assignments = _dal.Assignment
                    .ReadAll()
                    .Where(a => a.VolunteerId == volunteerId && a.EndTime != null)
                    .ToList();
            }
            return assignments;
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to get volunteer's assignment history.", ex);
        }
    }
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
}