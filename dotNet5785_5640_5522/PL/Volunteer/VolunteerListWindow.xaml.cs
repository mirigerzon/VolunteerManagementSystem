using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using BO;

namespace PL.Volunteer;
public partial class VolunteerListWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public VolunteerListWindow()
    {
        InitializeComponent();
        IsActiveFilterOptions = new List<bool?> { null, true, false };
        SelectedIsActiveFilter = null;
        SelectedSortField = VolunteerSortField.Id;
        DataContext = this;
    }
    public event PropertyChangedEventHandler PropertyChanged;
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
    private void LoadVolunteers()
    {
        VolunteerList = (SelectedSortField == VolunteerSortField.Id)
            ? s_bl.Volunteer.ReadAll(SelectedIsActiveFilter, null)
            : s_bl.Volunteer.ReadAll(SelectedIsActiveFilter, SelectedSortField);
    }
    private void VolunteerListObserver()
    {
        LoadVolunteers();
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(VolunteerListObserver);
        LoadVolunteers();
    }
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
    }
    private void EditVolunteer_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is VolunteerInList selected)
        {
            var fullVolunteer = s_bl.Volunteer.Read(selected.Id);
            var win = new VolunteerDetailsWindow(fullVolunteer);
            win.ShowDialog();
            LoadVolunteers();
        }
    }
    private void AddVolunteer_Click(object sender, RoutedEventArgs e)
    {
        var newVolunteer = new BO.Volunteer();
        var win = new VolunteerDetailsWindow(newVolunteer);
        win.ShowDialog();
        LoadVolunteers();
    }
}