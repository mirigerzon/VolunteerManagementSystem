using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BlApi;
using PL.Volunteer;

namespace PL;

/// Interaction logic for AdminMainWindow.xaml
/// This window serves as the main management dashboard for administrators.

public partial class AdminMainWindow : Window
{
    static readonly IBl s_bl = Factory.Get();
    private readonly BO.Volunteer _currentUser;
    private readonly DispatcherTimer _timer;

    // Track open child windows to enforce "at most one" rule
    private VolunteersListWindow? _volunteerListWindowInstance;
    private Window? _callListWindowInstance; // Assuming you'll create a CallListWindow
    public AdminMainWindow(BO.Volunteer currentUser)
    {
        InitializeComponent();
        UpdateRiskRangeDisplay();
        CurrentTime = s_bl.Admin.GetSystemClock();
        _currentUser = currentUser;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();

        // Set DataContext for CurrentTime binding
        this.DataContext = this;
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
            // In a real application, consider more robust error logging
            Console.WriteLine("Failed to advance system clock automatically: " + ex.Message);
        }
    }
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AdminMainWindow), new PropertyMetadata(DateTime.Now));
    private void RefreshCurrentTime()
    {
        CurrentTime = s_bl.Admin.GetSystemClock();
    }
    private void BtnAdvanceMinute_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Minute); RefreshCurrentTime(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error Advancing Time", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void BtnAdvanceHour_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Hour); RefreshCurrentTime(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error Advancing Time", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void BtnAdvanceDay_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Day); RefreshCurrentTime(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error Advancing Time", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void BtnAdvanceYear_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(BO.TimeUnit.Year); RefreshCurrentTime(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Error Advancing Time", MessageBoxButton.OK, MessageBoxImage.Error); }
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
                RefreshCurrentTime(); // Refresh time after DB init as it might reset the clock
                UpdateRiskRangeDisplay(); // Refresh risk range display
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to initialize DB: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null; // Restore normal cursor
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
                RefreshCurrentTime(); // Refresh time after DB reset as it might reset the clock
                UpdateRiskRangeDisplay(); // Refresh risk range display
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
        catch (FormatException)
        {
            MessageBox.Show("Invalid time span format. Please enter in format hh:mm or mm (e.g., 01:30 or 90).", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while updating risk range: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void UpdateRiskRangeDisplay()
    {
        try
        {
            TimeSpan span = s_bl.Admin.GetRiskTimeSpan();
            CurrentRiskRangeText = $"({FormatRiskRange(span)})";
        }
        catch (Exception ex)
        {
            CurrentRiskRangeText = "(N/A)";
            Console.WriteLine("Error loading risk time span: " + ex.Message);
        }
    }
    public string CurrentRiskRangeText
    {
        get { return (string)GetValue(CurrentRiskRangeTextProperty); }
        set { SetValue(CurrentRiskRangeTextProperty, value); }
    }
    public static readonly DependencyProperty CurrentRiskRangeTextProperty =
        DependencyProperty.Register("CurrentRiskRangeText", typeof(string), typeof(AdminMainWindow), new PropertyMetadata("(N/A)"));
    private string FormatRiskRange(TimeSpan span)
    {
        if (span.TotalHours >= 1)
            return $"{(int)span.TotalHours}h {span.Minutes}m";
        else
            return $"{span.Minutes}m";
    }
    private void BtnHandleVolunteers_Click(object sender, RoutedEventArgs e)
    {
        if (_volunteerListWindowInstance == null || !_volunteerListWindowInstance.IsLoaded)
        {
            _volunteerListWindowInstance = new VolunteersListWindow(_currentUser);
            _volunteerListWindowInstance.Closed += (s, args) => _volunteerListWindowInstance = null; // Clear instance on close
            _volunteerListWindowInstance.Show();
        }
        else
        {
            _volunteerListWindowInstance.Activate(); // Bring existing window to front
        }
    }
    private void BtnHandleCalls_Click(object sender, RoutedEventArgs e)
    {

        if (_callListWindowInstance == null || !_callListWindowInstance.IsLoaded)
        {
            _callListWindowInstance = new Call.CallsListWindow(_currentUser);
            _callListWindowInstance.Closed += (s, args) => _callListWindowInstance = null;
            _callListWindowInstance.Show();
        }
        else
        {
            _callListWindowInstance.Activate();
        }
    }
    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        this.Close();
    }
}