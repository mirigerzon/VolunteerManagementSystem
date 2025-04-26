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
    public IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive = null, VolunteerSortField? sortBy = null)
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
        });
    }
    public BO.Volunteer GetVolunteer(int id)
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
                Role = (DO.Enums.RoleEnum)volunteer.Role,
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
    public void UpdateVolunteer(int id, BO.Volunteer volunteer)
    {
        try
        {
            var dalVolunteer = _dal.Volunteer.Read(volunteer.Id);
            if (dalVolunteer == null)
                throw new BlDoesNotExistException("Volunteer not found");
            if (dalVolunteer.Id == volunteer.Id || dalVolunteer.Role == DO.Enums.RoleEnum.Mentor)
            {
                if (dalVolunteer.Role != volunteer.Role && dalVolunteer.Role != DO.Enums.RoleEnum.Mentor)
                    throw new BlInvalidException("Volunteer cannot change roles!");
                string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
                if (checkValues == "true")
                {
                    var volunteerToUpdate = Helpers.VolunteerManager.ConvertBoToDo(volunteer);
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
    public void RemoveVolunteer(int id)
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
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
            if (checkValues == "true")
            {
                var newVolunteer = Helpers.VolunteerManager.ConvertBoToDo(volunteer);
                _dal.Volunteer.Create(newVolunteer);
            }
            else
                throw new BlInvalidException(checkValues + " - this field is not valid");
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

#region לפני זריקת שגיאות
//using BO;
//using DalApi;
//using BlApi;

//namespace BlImplementation;

//public class VolunteerImplementation : BlApi.IVolunteer
//{       
//    private readonly IDal _dal = DalApi.Factory.Get;
//    public UserRole Login(string username, string password)
//    {
//        try
//        {
//            var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.FullName == username && v.Password == password);
//            if (volunteer == null)
//                throw new Exception("Invalid credentials");
//            UserRole role = (UserRole)volunteer.Role;
//            return role;
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("Login failed", ex);
//        }
//    }

//    //GetVolunteers: Retrieve list of volunteers with optional filters.
//    public IEnumerable<BO.VolunteerInList> GetVolunteers(bool? isActive = null, VolunteerSortField? sortBy = null)
//    {
//        var volunteers = _dal.Volunteer.ReadAll();
//        if (isActive.HasValue)
//            volunteers = volunteers.Where(v => v.IsActive == isActive.Value).ToList();
//        try
//        {
//            if (sortBy.HasValue)
//            {
//                switch (sortBy.Value)
//                {
//                    case VolunteerSortField.FullName:
//                        volunteers = volunteers.OrderBy(v => v.FullName).ToList();
//                        break;
//                    case VolunteerSortField.Distance:
//                        volunteers = volunteers.OrderBy(v => v.MaxOfDistance).ToList();
//                        break;
//                    case VolunteerSortField.Role:
//                        volunteers = volunteers.OrderBy(v => v.Role).ToList();
//                        break;
//                    case VolunteerSortField.IsActive:
//                        volunteers = volunteers.OrderBy(v => v.IsActive).ToList();
//                        break;
//                    default:
//                        throw new Exception("Sorting failed - Cannot sort by " + sortBy.Value);
//                }
//            }
//            else
//            {
//                volunteers = volunteers.OrderBy(v => v.Id).ToList();
//            }
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("ERROR", ex);
//        }

//        return volunteers.Select(v => new BO.VolunteerInList
//        {
//            Id = v.Id,
//            FullName = v.FullName,
//            IsActive = v.IsActive,
//            TotalHandledCalls = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.Treated),
//            TotalCanceledCalls = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.SelfCancelled),
//            ExpiredCallsCount = Helpers.VolunteerManager.TotalCallsByEndStatus(v.Id, DO.Enums.TerminationTypeEnum.Expired)
//        });
//    }

//    //GetVolunteer: Get full details of a specific volunteer.
//    public BO.Volunteer GetVolunteer(int id)
//    {
//        try
//        {
//            var volunteer = _dal.Volunteer.Read(id);
//            if (volunteer == null)
//                throw new Exception("Volunteer not found");
//            return new BO.Volunteer
//            {
//                Id = volunteer.Id,
//                FullName = volunteer.FullName,
//                PhoneNumber = volunteer.PhoneNumber,
//                Email = volunteer.Email,
//                Password = volunteer.Password,
//                Address = volunteer.Address,
//                Latitude = volunteer.Latitude,
//                Longitude = volunteer.Longitude,
//                Role = (DO.Enums.RoleEnum)volunteer.Role,
//                IsActive = volunteer.IsActive,
//                MaxDistance = volunteer.MaxOfDistance,
//                TypeOfDistance = (TypeOfDistance)volunteer.TypeOfDistance,
//                CurrentCall = null
//            };
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("ERROR", ex);
//        }
//    }

//    //UpdateVolunteer: Update volunteer information.
//    public void UpdateVolunteer(int id, BO.Volunteer volunteer)
//    {
//        try
//        {
//            var dalVolunteer = _dal.Volunteer.Read(volunteer.Id);
//            if (dalVolunteer == null)
//                throw new Exception("Volunteer not found");
//            if (dalVolunteer.Id == volunteer.Id || dalVolunteer.Role == DO.Enums.RoleEnum.Mentor)
//            {
//                if (dalVolunteer.Role != volunteer.Role && dalVolunteer.Role != DO.Enums.RoleEnum.Mentor)
//                    throw new Exception("Volunteer can not change roles!");
//                string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
//                if (checkValues == "true")
//                {
//                    var volunteerToUpdate = Helpers.VolunteerManager.ConvertBoToDo(volunteer);
//                    _dal.Volunteer.Create(volunteerToUpdate);
//                }
//                else
//                    throw new Exception(checkValues + " - this field is not valid");
//            }
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("Error updating volunteer", ex);
//        }
//    }

//    //RemoveVolunteer: Deactivate volunteer.
//    public void RemoveVolunteer(int id)
//    {
//        try
//        {
//            var volunteer = _dal.Volunteer.Read(id);
//            if (volunteer == null)
//                throw new Exception("Volunteer not found");
//            if (volunteer.IsActive == false || Helpers.VolunteerManager.TotalCallsByEndStatus(volunteer.Id, DO.Enums.TerminationTypeEnum.Treated) == 0)
//                throw new Exception("you can not delete this volunteer");
//            else
//                _dal.Volunteer.Delete(volunteer.Id);
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("Error deactivating volunteer", ex);
//        }
//    }

//    //AddVolunteer: Add a new volunteer.
//    public void AddVolunteer(BO.Volunteer volunteer)
//    {
//        try
//        {
//            string checkValues = Helpers.VolunteerManager.IsValid(volunteer);
//            if (checkValues == "true")
//            {
//                var newVolunteer = Helpers.VolunteerManager.ConvertBoToDo(volunteer);
//                _dal.Volunteer.Create(newVolunteer);
//            }
//            else
//                throw new Exception(checkValues + " - this field is not valid");
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("Error adding volunteer", ex);
//        }
//    }
//}
#endregion
