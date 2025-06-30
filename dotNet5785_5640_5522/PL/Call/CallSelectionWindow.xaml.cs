// CallSelectionWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BO;

namespace PL.Volunteer
{
    public partial class CallSelectionWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();

        public event PropertyChangedEventHandler PropertyChanged;

        public BO.Volunteer Volunteer { get; set; }

        public ObservableCollection<OpenCallInList> OpenCalls { get; set; }

        private OpenCallInList selectedCall;
        public OpenCallInList SelectedCall
        {
            get => selectedCall;
            set
            {
                selectedCall = value;
                OnPropertyChanged(nameof(SelectedCall));
                OnPropertyChanged(nameof(Description));
            }
        }

        public CallInProgress SelectedAssignment { get; set; }

        public string Description => SelectedCall?.Description;

        public CallSelectionWindow(BO.Volunteer volunteer)
        {
            Volunteer = volunteer;
            OpenCalls = new ObservableCollection<OpenCallInList>(bl.Call.GetOpenCallsForVolunteer(volunteer.Id));
            InitializeComponent();
            DataContext = this;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedCall != null)
                {
                    bl.Call.RequestAssignmentTreatment(SelectedCall.Id, Volunteer.Id);
                    MessageBox.Show("Call assigned successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private volatile DispatcherOperation? _observerOperation = null;

        public void RefreshOpenCalls()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(new Action(() =>
                {
                    var updatedCalls = bl.Call.GetOpenCallsForVolunteer(Volunteer.Id);
                    OpenCalls.Clear();
                    foreach (var call in updatedCalls)
                    {
                        OpenCalls.Add(call);
                    }
                }));
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedCall));
            OnPropertyChanged(nameof(Description));
            // TODO: Update map
        }

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void CallsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedCall));
            OnPropertyChanged(nameof(Description));
        }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is OpenCallInList clickedCall)
                {
                    SelectedCall = clickedCall;
                }
                if (SelectedCall == null)
                {
                    MessageBox.Show("Please select a call from the list", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var assignment = bl.Call.RequestAssignmentTreatment(Volunteer.Id, SelectedCall.Id);
                SelectedAssignment = assignment;
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}