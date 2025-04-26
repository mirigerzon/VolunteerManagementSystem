using BlApi;

namespace BlImplementation;

internal class Bl : IBl
{
    public IAdmin Admin { get; } = new AdminImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();
}
