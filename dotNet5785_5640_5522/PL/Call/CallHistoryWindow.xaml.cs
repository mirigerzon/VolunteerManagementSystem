using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class CallHistoryWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
        public BO.Volunteer Volunteer { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        private IEnumerable<ClosedCallInList> _closedCalls;
        public IEnumerable<ClosedCallInList> ClosedCalls
        {
            get => _closedCalls;
            set { _closedCalls = value; OnPropertyChanged(nameof(ClosedCalls)); }
        }
            
        public CallHistoryWindow(BO.Volunteer volunteer)
        {
            Volunteer = volunteer;
            ClosedCalls = bl.Call.GetClosedCallsOfVolunteer(volunteer.Id);
            InitializeComponent();
            DataContext = this;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
