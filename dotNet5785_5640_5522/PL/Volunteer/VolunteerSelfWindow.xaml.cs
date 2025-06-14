using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteerSelfWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
        public event PropertyChangedEventHandler PropertyChanged;

        public BO.Volunteer Volunteer { get; set; }
        public BO.Call? ActiveCall { get; set; }
        public BO.OpenCallInList? ActiveAssignment { get; set; }

        public bool HasActiveCall => ActiveAssignment != null;
        public bool CanChooseCall => !HasActiveCall && Volunteer.IsActive;
        public bool CanChangeActivity => !HasActiveCall;
        public Array TypeOfDistanceValues => Enum.GetValues(typeof(TypeOfDistance));

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // רדיוס כדור הארץ בק"מ
            var latRad1 = DegreesToRadians(lat1);
            var latRad2 = DegreesToRadians(lat2);
            var deltaLat = DegreesToRadians(lat2 - lat1);
            var deltaLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(latRad1) * Math.Cos(latRad2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // המרחק בק"מ
        }

        public double ActiveCallDistance
        {
            get
            {
                if (Volunteer == null || ActiveCall == null ||
                    Volunteer.Latitude == null || Volunteer.Longitude == null ||
                    ActiveCall.Latitude == null || ActiveCall.Longitude == null)
                    return 0.0;

                return CalculateDistance(
                    Volunteer.Latitude.Value, Volunteer.Longitude.Value,
                    ActiveCall.Latitude.Value, ActiveCall.Longitude.Value);
            }
        }


        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public VolunteerSelfWindow(BO.Volunteer volunteer)
        {
            Volunteer = volunteer;

            try
            {
                var activeAssignments = bl.Call.GetOpenCallsForVolunteer(Volunteer.Id);
                if (activeAssignments != null && activeAssignments.Count > 0)
                {
                    ActiveAssignment = activeAssignments[0];
                    ActiveCall = bl.Call.Read(ActiveAssignment.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בטעינת הקריאה הפעילה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeComponent();
            DataContext = this;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Volunteer.Password = ((PasswordBox)sender).Password;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.Volunteer.Update(Volunteer.Id, Volunteer);
                MessageBox.Show("הפרטים עודכנו בהצלחה", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveAssignment == null) return;

            try
            {
                bl.Call.UpdateEndTreatment(Volunteer.Id, ActiveAssignment.Id);
                MessageBox.Show("הקריאה סומנה כסגורה", "סיום טיפול", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveAssignment == null) return;

            try
            {
                bl.Call.CancelAssignmentTreatment(Volunteer.Id, ActiveAssignment.Id);
                MessageBox.Show("הטיפול בקריאה בוטל", "ביטול טיפול", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            var callSelectWindow = new CallSelectionWindow(Volunteer);
            callSelectWindow.ShowDialog();
            Close();
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new CallHistoryWindow(Volunteer);
            historyWindow.ShowDialog();
        }

        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
