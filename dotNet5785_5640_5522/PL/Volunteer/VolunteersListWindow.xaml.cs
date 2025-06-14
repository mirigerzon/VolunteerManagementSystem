using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;

namespace PL.Volunteer;

public partial class VolunteersListWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private readonly BO.Volunteer _currentUser;
    public VolunteersListWindow(BO.Volunteer currentUser)
    {
        InitializeComponent();
        this.Title = $"Volunteer List - {currentUser.FullName} ({currentUser.Id})";
        _currentUser = currentUser;
        IsActiveFilterOptions = new List<bool?> { null, true, false };
        SelectedIsActiveFilter = null;
        SelectedSortField = VolunteerSortField.Id;
        DataContext = this;
    }
    public event PropertyChangedEventHandler PropertyChanged;
    public Array Roles => Enum.GetValues(typeof(DO.Enums.RoleEnum));

    private void OnPropertyChanged(string propName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    private IEnumerable<VolunteerInList> _volunteerList;
    public IEnumerable<VolunteerInList> VolunteerList
    {
        get => _volunteerList;
        set
        {
            _volunteerList = value;
            OnPropertyChanged(nameof(VolunteerList));
        }
    }
    public List<bool?> IsActiveFilterOptions { get; }

    private bool? _selectedIsActiveFilter;
    public bool? SelectedIsActiveFilter
    {
        get => _selectedIsActiveFilter;
        set
        {
            if (_selectedIsActiveFilter != value)
            {
                _selectedIsActiveFilter = value;
                OnPropertyChanged(nameof(SelectedIsActiveFilter));
                LoadVolunteers();
            }
        }
    }

    private VolunteerSortField _selectedSortField = VolunteerSortField.Id;
    public VolunteerSortField SelectedSortField
    {
        get => _selectedSortField;
        set
        {
            if (_selectedSortField != value)
            {
                _selectedSortField = value;
                OnPropertyChanged(nameof(SelectedSortField));
                LoadVolunteers();
            }
        }
    }

    private VolunteerInList? _selectedVolunteer;
    public VolunteerInList? SelectedVolunteer
    {
        get => _selectedVolunteer;
        set
        {
            if (_selectedVolunteer != value)
            {
                _selectedVolunteer = value;
                OnPropertyChanged(nameof(SelectedVolunteer));
            }
        }
    }
    private void LoadVolunteers()
    {
        try
        {
            VolunteerList = (SelectedSortField == VolunteerSortField.Id)
                ? s_bl.Volunteer.ReadAll(SelectedIsActiveFilter, null)
                : s_bl.Volunteer.ReadAll(SelectedIsActiveFilter, SelectedSortField);
        }
        catch
        {
            MessageBox.Show("An error occurred while loading the volunteer list. Please try again later.",
                            "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void VolunteerListObserver() => LoadVolunteers();
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(VolunteerListObserver);
        LoadVolunteers();
    }
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
    }
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
        {
            try
            {
                var fullVolunteer = s_bl.Volunteer.Read(SelectedVolunteer.Id);
                var win = new VolunteerDetailsWindow(fullVolunteer, _currentUser);
                win.ShowDialog();
                LoadVolunteers();
            }
            catch
            {
                MessageBox.Show("An error occurred while opening the volunteer details.",
                                "View Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private void AddVolunteer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var newVolunteer = new BO.Volunteer();
            var win = new VolunteerDetailsWindow(newVolunteer, _currentUser);
            win.ShowDialog();
            LoadVolunteers();
        }
        catch
        {
            MessageBox.Show("An error occurred while trying to add a new volunteer.",
                            "Add Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
