// CallSelectionWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BO;

namespace PL.Volunteer
{
    public partial class CallSelectionWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
        public event PropertyChangedEventHandler PropertyChanged;
        public BO.Volunteer Volunteer { get; set; }
        public IEnumerable<OpenCallInList> OpenCalls { get; set; }
        public OpenCallInList SelectedCall { get; set; }
        public string Description => SelectedCall?.Description;

        public CallSelectionWindow(BO.Volunteer volunteer)
        {
            Volunteer = volunteer;
            OpenCalls = bl.Call.GetOpenCallsForVolunteer(volunteer.Id);
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
                    MessageBox.Show("הקריאה הוקצתה בהצלחה", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedCall));
            OnPropertyChanged(nameof(Description));
            // TODO: עדכון מפה
        }

        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void CallsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
        }

    }
}
