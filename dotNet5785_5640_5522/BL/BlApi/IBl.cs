using Helpers;

namespace BlApi;
public interface IBl
{
    IAdmin Admin { get; }
    IVolunteer Volunteer { get; }
    ICall Call { get; }
}