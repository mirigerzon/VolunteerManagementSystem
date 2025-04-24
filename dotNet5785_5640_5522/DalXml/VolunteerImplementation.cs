namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    // Converts an XML element to a Volunteer object.
    static Volunteer GetVolunteer(XElement v)
    {
        return new DO.Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("Invalid ID format"),
            FullName = (string?)v.Element("FullName") ?? "",
            PhoneNumber = (string?)v.Element("PhoneNumber") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            Password = (string?)v.Element("Password") ?? "",
            Address = (string?)v.Element("Address") ?? "",
            Longitude = v.ToDoubleNullable("Longitude") ?? 0,
            Latitude = v.ToDoubleNullable("Latitude") ?? 0, 
            IsActive = (bool?)v.Element("IsActive") ?? false,
            MaxOfDistance = v.ToDoubleNullable("MaxOfDistance") ?? 0
        };
    }
    // Creates a collection of XML elements representing a Volunteer.
    private static IEnumerable<XElement> CreateVolunteerElements(Volunteer volunteer)
    {
        yield return new XElement("Id", volunteer.Id);
        yield return new XElement("FullName", volunteer.FullName);
        yield return new XElement("PhoneNumber", volunteer.PhoneNumber);
        yield return new XElement("Email", volunteer.Email);
        yield return new XElement("Password", volunteer.Password);
        yield return new XElement("Address", volunteer.Address);
        yield return new XElement("Longitude", volunteer.Longitude);
        yield return new XElement("Latitude", volunteer.Latitude);
        yield return new XElement("IsActive", volunteer.IsActive);
        yield return new XElement("MaxOfDistance", volunteer.MaxOfDistance);
    }
    // Adds a new Volunteer to the XML storage.
    public void Create(Volunteer volunteer)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);

        if (volunteersRootElem.Elements().Any(vo => (int?)vo.Element("Id") == volunteer.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID {volunteer.Id} already exists.");

        volunteersRootElem.Add(new XElement("Volunteer", CreateVolunteerElements(volunteer)));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_Volunteers_xml);
    }
    // Deletes a Volunteer by ID from the XML storage.
    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);
        XElement? volunteerElement = volunteersRootElem.Elements()
            .FirstOrDefault(vo => (int?)vo.Element("Id") == id) ?? throw new DalDoesNotExistException($"Volunteer with ID {id} not found.");
        volunteerElement.Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_Volunteers_xml);
    }
    // Deletes all Volunteers from the XML storage.
    public void DeleteAll()
    {
        XElement newRootElement = new("ArrayOfVolunteer");
        XMLTools.SaveListToXMLElement(newRootElement, Config.s_Volunteers_xml);
    }
    // Reads and retrieves a Volunteer by ID from the XML storage.
    public Volunteer? Read(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);
        XElement? volunteerElement = volunteersRootElem.Elements()
            .FirstOrDefault(vo => (int?)vo.Element("Id") == id);

        return volunteerElement != null ? GetVolunteer(volunteerElement) : null;
    }
    // Reads and retrieves the first Volunteer matching a filter condition.
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return ReadAll().FirstOrDefault(filter);
    }
    // Reads and retrieves all Volunteers or those matching a filter condition.
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);
        var volunteers = volunteersRootElem.Elements()
            .Select(e => GetVolunteer(e));

        return filter == null ? volunteers : volunteers.Where(filter);
    }
    // Updates a Volunteer in the XML storage.
    public void Update(Volunteer volunteer)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_Volunteers_xml);
        XElement? volunteerElement = volunteersRootElem.Elements()
            .FirstOrDefault(vo => (int?)vo.Element("Id") == volunteer.Id) ?? throw new DalDoesNotExistException($"Volunteer with ID {volunteer.Id} not found.");
        volunteerElement.ReplaceWith(new XElement("Volunteer", CreateVolunteerElements(volunteer)));
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_Volunteers_xml);
    }
}