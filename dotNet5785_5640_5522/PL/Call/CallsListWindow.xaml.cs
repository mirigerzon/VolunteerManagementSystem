using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;
using DO;

namespace PL.Call
{
    public partial class CallsListWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private readonly BO.Volunteer _currentUser;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

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

            IsActiveFilterOptions = new List<bool?> { null, true, false };
            SelectedIsActiveFilter = null;

            SelectedSortField = CallFieldFilter.Id;
        }

        private IEnumerable<CallInList> _callList;
        public IEnumerable<CallInList> CallList
        {
            get => _callList;
            set { _callList = value; OnPropertyChanged(nameof(CallList)); }
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

        private CallFieldFilter _selectedSortField;
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
                    filteredCalls = _selectedIsActiveFilter.Value
                        ? filteredCalls.Where(call => call.Status is CallStatus.Open or CallStatus.InTreatment or CallStatus.OpenAtRisk or CallStatus.InTreatmentAtRisk)
                        : filteredCalls.Where(call => call.Status is CallStatus.Closed or CallStatus.Expired);
                }

                if (_selectedCallStatusFilter.HasValue)
                    filteredCalls = filteredCalls.Where(call => call.Status == _selectedCallStatusFilter.Value);

                if (_selectedCallTypeFilter.HasValue)
                    filteredCalls = filteredCalls.Where(call => call.CallType == _selectedCallTypeFilter.Value);

                CallList = filteredCalls.ToList();
            }
            catch (BO.BlDependencyNotInitializedException ex)
            {
                MessageBox.Show($"Dependency not initialized: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CallList = new List<CallInList>();
            }
            catch (BO.BlXMLFileLoadCreateException ex)
            {
                MessageBox.Show($"XML file error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CallList = new List<CallInList>();
            }
            catch (BO.BlGeneralException ex)
            {
                MessageBox.Show($"General logical error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CallList = new List<CallInList>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (sender is DataGrid dg && dg.SelectedItem is CallInList selectedCall)
            {
                try
                {
                    var fullCall = s_bl.Call.Read(selectedCall.CallId);
                    var win = new CallDetailsWindow(fullCall);
                    win.Closed += (s, args) => LoadCalls();
                    win.ShowDialog();
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show(ex.Message, "Call Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCalls();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening call details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall != null)
            {
                var result = MessageBox.Show($"Delete call number {SelectedCall.Id}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.Delete(SelectedCall.CallId);
                        MessageBox.Show("Call deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCalls();
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        MessageBox.Show(ex.Message, "Not Found for Deletion", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCalls();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Deletion error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a call to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall == null)
            {
                MessageBox.Show("Select a call to cancel assignment.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                
                var call = s_bl.Call.Read(SelectedCall.CallId);
                var volunteer = s_bl.Volunteer.ReadAll().FirstOrDefault(v => v.CurrentCallId == call.Id);
                if (volunteer != null)
                {

                    s_bl.Call.CancelAssignmentTreatment(_currentUser.Id, SelectedCall.Id);
                    s_bl.Call.Update(new BO.Call
                    {
                        Id = call.Id,
                        Type = call.Type,
                        Description = call.Description,
                        CallerAddress = call.CallerAddress,
                        Latitude = call.Latitude,
                        Longitude = call.Longitude,
                        StartTime = call.StartTime,
                        MaxEndTime = call.MaxEndTime,
                        Status = BO.CallStatus.Open
                    });
                    MessageBox.Show("Treatment cancelled successfully.", "Cancel Assignment", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedCall = null;
                    LoadCalls();
                }
                else
                {
                    MessageBox.Show("No volunteer found handling this call.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling assignment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}