using BO;
using DalApi;
using BlApi;

namespace BlImplementation;
public class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly IDal _dal = DalApi.Factory.Get;
    public UserRole Login(string username, string password)
    {
        try
        {
            var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.FullName == username && v.Password == password);
            if (volunteer == null)
                throw new BlDoesNotExistException("Invalid credentials");
            UserRole role = (UserRole)volunteer.Role;
            return role;
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Login failed", ex);
        }
    }
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, VolunteerSortField? sortBy = null)
    {
        var volunteers = _dal.Volunteer.ReadAll();
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
                    case VolunteerSortField.Distance:
                        volunteers = volunteers.OrderBy(v => v.MaxOfDistance).ToList();
                        break;
                    case VolunteerSortField.Role:
                        volunteers = volunteers.OrderBy(v => v.Role).ToList();
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

        return volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            IsActive = v.IsActive,
            TotalHandledCalls = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.Treated),
            TotalCanceledCalls = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.SelfCancelled),
            ExpiredCallsCount = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.Expired)
        }).ToList();
    }
    public BO.Volunteer Read(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id);
            if (volunteer == null)
                throw new BlDoesNotExistException("Volunteer not found");
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
                CurrentCall = null
            };
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Error retrieving volunteer.", ex);
        }
    }
    public void Update(int id, BO.Volunteer volunteer)
    {
        try
        {
            var dalVolunteer = _dal.Volunteer.Read(volunteer.Id);
            if (dalVolunteer == null)
                throw new BlDoesNotExistException("Volunteer not found");
            if (dalVolunteer.Id == volunteer.Id || dalVolunteer.Role == DO.Enums.RoleEnum.Manager)
            {
                if ((UserRole)dalVolunteer.Role != volunteer.Role && (UserRole)dalVolunteer.Role != UserRole.Manager)
                    throw new BlInvalidException("Volunteer cannot change roles!");
                string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
                if (checkValues == "true")
                {
                    var volunteerToUpdate = Helpers.VolunteerManager.ConvertBoToDo(volunteer);
                    _dal.Volunteer.Delete(id);
                    _dal.Volunteer.Create(volunteerToUpdate);
                }
                else
                    throw new BlInvalidException(checkValues + " - this field is not valid");
            }
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Error updating volunteer.", ex);
        }
    }
    public void Delete(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id);
            if (volunteer == null)
                throw new BlDoesNotExistException("Volunteer not found");
            if (!volunteer.IsActive || Helpers.VolunteerManager.TotalCallsByEndStatus(volunteer.Id, DO.Enums.TerminationTypeEnum.Treated) == 0)
                throw new BlInvalidException("You cannot delete this volunteer");
            else
                _dal.Volunteer.Delete(volunteer.Id);
        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Error deactivating volunteer.", ex);
        }
    }
    public void Create(BO.Volunteer volunteer)
    {
        try
        {
            string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
            if (checkValues == "true")
                if (true)
            {
                var newVolunteer = Helpers.VolunteerManager.ConvertBoToDo(volunteer);
                _dal.Volunteer.Create(newVolunteer);
            }
            else
                throw new BlInvalidException("checkValues" + " - this field is not valid");
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
}