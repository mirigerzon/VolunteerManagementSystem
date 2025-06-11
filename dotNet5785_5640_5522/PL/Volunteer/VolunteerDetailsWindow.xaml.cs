using System;
using System.ComponentModel;
using System.Windows;
using BO;

namespace PL.Volunteer;

public partial class VolunteerDetailsWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private readonly BO.Volunteer _currentUser;
    public Array TypeOfDistanceValues => Enum.GetValues(typeof(BO.TypeOfDistance));
    public VolunteerDetailsWindow(BO.Volunteer volunteer, BO.Volunteer currentUser)
    {
        Volunteer = volunteer;
        _currentUser = currentUser;
        ButtonText = volunteer.Id == 0 ? "Add" : "Update";

        InitializeComponent();
        DataContext = this;

        if (Volunteer.Id != 0)
            s_bl.Volunteer.AddObserver(Volunteer.Id, VolunteerObserver);

        this.Closed += (s, e) =>
        {
            if (Volunteer.Id != 0)
                s_bl.Volunteer.RemoveObserver(Volunteer.Id, VolunteerObserver);
        };
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        _currentUser.FullName = LastNameTextBox.Text.Trim();
        _currentUser.PhoneNumber = PhoneTextBox.Text.Trim();
        _currentUser.IsActive = IsActiveCheckBox.IsChecked == true;
    }

    private void OnPropertyChanged(string propName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    private BO.Volunteer _volunteer;
    public BO.Volunteer Volunteer
    {
        get => _volunteer;
        set
        {
            _volunteer = value;
            OnPropertyChanged(nameof(Volunteer));
        }
    }
    private string _buttonText;
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            OnPropertyChanged(nameof(ButtonText));
        }
    }
    private void BtnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Volunteer.Password = passwordBox.Password;
            if (ButtonText == "Add")
            {
                s_bl.Volunteer.Create(Volunteer);
                MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else // Update
            {
                s_bl.Volunteer.Update(_currentUser.Id, Volunteer);
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void VolunteerObserver()
    {
        if (Volunteer?.Id != 0)
        {
            int id = Volunteer.Id;
            Volunteer = null;
            Volunteer = s_bl.Volunteer.Read(id);
        }
    }
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUser?.Id == 0)
            return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete volunteer {_currentUser.Id}?",
            "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                s_bl.Volunteer.Delete(_currentUser.Id);
                MessageBox.Show("Volunteer deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while trying to delete the volunteer:\n{ex.Message}",
                                "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }


}
