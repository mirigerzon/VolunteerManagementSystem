using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;
using DO;

namespace PL.Call;

public partial class CallsListWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private readonly BO.Volunteer _currentUser;
    public CallsListWindow(BO.Volunteer currentUser)
    {
        InitializeComponent();
        _currentUser = currentUser;
        CallStatusOptions = Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus?>().ToList();
        CallStatusOptions.Insert(0, null);
        SelectedCallStatusFilter = null;

        CallTypeOptions = Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType?>().ToList();
        CallTypeOptions.Insert(0, null);
        SelectedCallTypeFilter = null;

        SelectedSortField = CallFieldFilter.Id;

        IsActiveFilterOptions = new List<bool?> { null, true, false };
        SelectedIsActiveFilter = null;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    private IEnumerable<CallInList> _callList;
    public IEnumerable<CallInList> CallList
    {
        get => _callList;
        set
        {
            _callList = value;
            OnPropertyChanged(nameof(CallList));
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
                LoadCalls();
            }
        }
    }
    public List<BO.CallStatus?> CallStatusOptions { get; }
    private BO.CallStatus? _selectedCallStatusFilter;
    public BO.CallStatus? SelectedCallStatusFilter
    {
        get => _selectedCallStatusFilter;
        set
        {
            if (_selectedCallStatusFilter != value)
            {
                _selectedCallStatusFilter = value;
                OnPropertyChanged(nameof(SelectedCallStatusFilter));
                LoadCalls();
            }
        }
    }
    public List<BO.CallType?> CallTypeOptions { get; }
    private BO.CallType? _selectedCallTypeFilter;
    public BO.CallType? SelectedCallTypeFilter
    {
        get => _selectedCallTypeFilter;
        set
        {
            if (_selectedCallTypeFilter != value)
            {
                _selectedCallTypeFilter = value;
                OnPropertyChanged(nameof(SelectedCallTypeFilter));
                LoadCalls();
            }
        }
    }
    private CallFieldFilter _selectedSortField = CallFieldFilter.Id;
    public CallFieldFilter SelectedSortField
    {
        get => _selectedSortField;
        set
        {
            if (_selectedSortField != value)
            {
                _selectedSortField = value;
                OnPropertyChanged(nameof(SelectedSortField));
                LoadCalls();
            }
        }
    }
    private CallInList? _selectedCall;
    public CallInList? SelectedCall
    {
        get => _selectedCall;
        set
        {
            if (_selectedCall != value)
            {
                _selectedCall = value;
                OnPropertyChanged(nameof(SelectedCall));
            }
        }
    }
    private void LoadCalls()
    {
        try
        {
            IEnumerable<CallInList> allCalls = s_bl.Call.GetCallsList(null, null, _selectedSortField);
            IEnumerable<CallInList> filteredCalls = allCalls;
            if (_selectedIsActiveFilter.HasValue)
            {
                if (_selectedIsActiveFilter.Value)
                {
                    filteredCalls = filteredCalls.Where(call =>
                        call.Status == CallStatus.Open ||
                        call.Status == CallStatus.InTreatment ||
                        call.Status == CallStatus.OpenAtRisk ||
                        call.Status == CallStatus.InTreatmentAtRisk);
                }
                else
                {
                    filteredCalls = filteredCalls.Where(call =>
                        call.Status == CallStatus.Closed ||
                        call.Status == CallStatus.Expired);
                }
            }

            if (_selectedCallStatusFilter.HasValue)
            {
                filteredCalls = filteredCalls.Where(call => call.Status == _selectedCallStatusFilter.Value);
            }

            if (_selectedCallTypeFilter.HasValue)
            {
                filteredCalls = filteredCalls.Where(call => call.CallType == _selectedCallTypeFilter.Value);
            }

            CallList = filteredCalls.ToList();
        }
        catch (BO.BlDependencyNotInitializedException ex)
        {
            MessageBox.Show($"Service dependency not initialized: {ex.Message}",
                                 "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CallList = new List<CallInList>();
        }
        catch (BO.BlXMLFileLoadCreateException ex)
        {
            MessageBox.Show($"Failed to load or create data file: {ex.Message}",
                                 "Data File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CallList = new List<CallInList>();
        }
        catch (BO.BlGeneralException ex)
        {
            MessageBox.Show($"An unexpected business logic error occurred: {ex.Message}",
                                 "Business Logic Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CallList = new List<CallInList>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unknown error occurred while loading the call list: {ex.Message}",
                                 "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CallList = new List<CallInList>();
        }
    }
    private void CallListObserver() => LoadCalls();
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(CallListObserver);
        LoadCalls();
    }
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CallListObserver);
    }
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // ישאר כפי שהוא, מכיוון שה-if (sender is DataGrid dg...) הוא חלק מ-DataBinding
        // וקבלת ה-selectedCall דרך dg.SelectedItem נחשבת נכונה.
        if (sender is DataGrid dg && dg.SelectedItem is CallInList selectedCall)
        {
            try
            {
                var fullCall = s_bl.Call.Read(selectedCall.Id);
                var win = new CallDetailsWindow(fullCall);
                win.Closed += (s, args) => LoadCalls();
                win.ShowDialog();
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Call Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCalls();
            }
            catch (BO.BlGeneralException ex)
            {
                MessageBox.Show($"An unexpected business logic error occurred: {ex.Message}",
                                 "Business Logic Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unknown error occurred while opening the call details: {ex.Message}",
                                 "View Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private void AddCall_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var newCall = new BO.Call();
            var win = new CallDetailsWindow(newCall);
            win.Closed += (s, args) => LoadCalls();
            win.ShowDialog();
        }
        catch (BO.BlAlreadyExistsException ex)
        {
            MessageBox.Show($"Cannot add call: {ex.Message}",
                                 "Addition Error - Already Exists", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlInvalidException ex)
        {
            MessageBox.Show($"Invalid call data: {ex.Message}",
                                 "Addition Error - Invalid Data", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlMissingDataException ex)
        {
            MessageBox.Show($"Missing required data: {ex.Message}",
                                 "Addition Error - Missing Data", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlGeneralException ex)
        {
            MessageBox.Show($"An unexpected business logic error occurred: {ex.Message}",
                                 "Business Logic Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unknown error occurred while trying to add a new call: {ex.Message}",
                                 "Add Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void DeleteCall_Click(object sender, RoutedEventArgs e)
    {
        // שינוי: שימוש ב-SelectedCall במקום גישה ל-sender.DataContext
        if (SelectedCall != null)
        {
            var callToDelete = SelectedCall;
            var result = MessageBox.Show($"Are you sure you want to delete call ID: {callToDelete.Id}?",
                                         "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.Delete(callToDelete.Id);
                    MessageBox.Show($"Call ID: {callToDelete.Id} deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCalls();
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(ex.Message, "Deletion Error - Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCalls();
                }
                catch (BO.BlInvalidException ex)
                {
                    MessageBox.Show($"Cannot delete call: {ex.Message}", "Deletion Error - Invalid State", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlGeneralException ex)
                {
                    MessageBox.Show($"An unexpected business logic error occurred: {ex.Message}",
                                         "Business Logic Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unknown error occurred during deletion: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("בבקשה בחר קריאה למחיקה.", "אין קריאה נבחרת", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    private void CancelAssignment_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedCall == null)
        {
            MessageBox.Show("בבקשה בחר קריאה לביטול הקצאה.", "אין קריאה נבחרת", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        int callIdToCancel = SelectedCall.Id;
        int currentLoggedInVolunteerId = _currentUser.Id;

        if (currentLoggedInVolunteerId == 0)
        {
            MessageBox.Show("שגיאה: לא ניתן לזהות את המתנדב המחובר.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        CancelAssignmentTreatment(callIdToCancel);
    }
    public void CancelAssignmentTreatment(int callId)
    {
        try
        {
            BO.Call? callObject = s_bl.Call.Read(callId);
            var volunteers = s_bl.Volunteer.ReadAll()
                 .Where(v => v.CurrentCallId == callObject?.Id);
            BO.Volunteer? volunteer = null;
            volunteers.ToList().ForEach(v =>
            {
                volunteer = s_bl.Volunteer.Read(v.Id);
            });
            if (volunteer != null)
            {
                var assignmentId = volunteer.CurrentCall.Id;
                s_bl.Call.CancelAssignmentTreatment(_currentUser.Id, assignmentId);
                MessageBox.Show($"Assignment ID: {assignmentId} cancelled successfully by volunteer {_currentUser.Id}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCalls();
            }
            else
                MessageBox.Show($"There is no assignment for call: {callObject?.Id}");

        }
        catch (Exception ex)
        {
            throw new BlInvalidException("Failed to cancel assignment treatment.", ex);
        }
    }
}