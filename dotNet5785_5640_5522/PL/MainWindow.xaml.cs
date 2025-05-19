using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BlApi;
using PL.Volunteer;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        private readonly DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();
            UpdateRiskRangeDisplay();
            CurrentTime = DateTime.Now;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            RefreshCurrentTime();
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute);
                RefreshCurrentTime();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to advance system clock automatically: " + ex.Message);
            }
        }
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
        private void RefreshCurrentTime()
        {
            CurrentTime = s_bl.Admin.GetSystemClock();
        }
        private void BtnAdvanceMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute);
            RefreshCurrentTime();
        }
        private void BtnAdvanceHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Hour);
            RefreshCurrentTime();
        }
        private void BtnAdvanceDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Day);
            RefreshCurrentTime();
        }
        private void BtnAdvanceYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Year);
            RefreshCurrentTime();
        }
        private void BtnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to initialize the database? This may overwrite existing data.",
                "Confirm Initialization",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.InitializeDatabase();
                    MessageBox.Show("Database initialized successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to initialize DB: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null; // החזר את הסמן הרגיל
                }
            }
        }
        private void BtnResetDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset the database? All data will be cleared.",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.ResetDatabase();
                    MessageBox.Show("Database reset successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to reset DB: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
        private void BtnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string riskValue = txtRiskRange.Text;
                s_bl.Admin.SetRiskTimeSpan(TimeSpan.Parse(riskValue));
                UpdateRiskRangeDisplay();
                txtRiskRange.Clear();
                MessageBox.Show($"Updated risk range: {riskValue}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid time span format. Please enter in format hh:mm or mm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdateRiskRangeDisplay()
        {
            try
            {
                TimeSpan span = s_bl.Admin.GetRiskTimeSpan();
                txtCurrentRiskRange.Text = $"({FormatRiskRange(span)})";
            }
            catch (Exception ex)
            {
                txtCurrentRiskRange.Text = "(N/A)";
                Console.WriteLine("Error loading risk time span: " + ex.Message);
            }
        }
        private string FormatRiskRange(TimeSpan span)
        {
            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours}h {span.Minutes}m";
            else
                return $"{span.Minutes}m";
        }
        private void BtnHandleVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }
    }
}
