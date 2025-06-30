// AdminMainWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BlApi;
using BO;
using PL.Volunteer;

namespace PL;

public partial class AdminMainWindow : Window
{
    static readonly IBl s_bl = Factory.Get();
    private readonly BO.Volunteer _currentUser;
    private volatile DispatcherOperation? _statusUpdateOperation = null;
    private readonly List<Window> _childWindows = new();

    public AdminMainWindow(BO.Volunteer currentUser)
    {
        InitializeComponent();
        _currentUser = currentUser;

        UpdateRiskRangeDisplay();
        CurrentTime = s_bl.Admin.GetSystemClock();
        UpdateStatusCounts();

        s_bl.Admin.AddClockObserver(OnClockUpdated);

        this.DataContext = this;
    }

    private void OnClockUpdated()
    {
        Dispatcher.Invoke(() =>
        {
            RefreshCurrentTime();
            UpdateStatusCounts();
        });
    }

    public DateTime CurrentTime
    {
        get => (DateTime)GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AdminMainWindow), new PropertyMetadata(DateTime.Now));

    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(AdminMainWindow), new PropertyMetadata(1));

    public int Interval
    {
        get => (int)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    public static readonly DependencyProperty IsSimulatorRunningProperty =
        DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(AdminMainWindow), new PropertyMetadata(false));

    public bool IsSimulatorRunning
    {
        get => (bool)GetValue(IsSimulatorRunningProperty);
        set => SetValue(IsSimulatorRunningProperty, value);
    }

    private void RefreshCurrentTime() => CurrentTime = s_bl.Admin.GetSystemClock();

    private void BtnAdvanceMinute_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(TimeUnit.Minute); RefreshCurrentTime(); UpdateStatusCounts(); }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void BtnAdvanceHour_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(TimeUnit.Hour); RefreshCurrentTime(); UpdateStatusCounts(); }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void BtnAdvanceDay_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(TimeUnit.Day); RefreshCurrentTime(); UpdateStatusCounts(); }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void BtnAdvanceYear_Click(object sender, RoutedEventArgs e)
    {
        try { s_bl.Admin.AdvanceSystemClock(TimeUnit.Year); RefreshCurrentTime(); UpdateStatusCounts(); }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void BtnInitializeDB_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Initialize database?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                s_bl.Admin.InitializeDatabase();
                RefreshCurrentTime(); UpdateRiskRangeDisplay(); UpdateStatusCounts();
                MessageBox.Show("Initialization successful.");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }

    private void BtnResetDB_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Complete reset?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            try
            {
                s_bl.Admin.ResetDatabase();
                RefreshCurrentTime(); UpdateRiskRangeDisplay(); UpdateStatusCounts();
                MessageBox.Show("Reset successful.");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
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
            MessageBox.Show("Updated successfully");
        }
        catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
    }

    private void UpdateRiskRangeDisplay()
    {
        try { CurrentRiskRangeText = $"({FormatRiskRange(s_bl.Admin.GetRiskTimeSpan())})"; }
        catch { CurrentRiskRangeText = "(N/A)"; }
    }

    public string CurrentRiskRangeText
    {
        get => (string)GetValue(CurrentRiskRangeTextProperty);
        set => SetValue(CurrentRiskRangeTextProperty, value);
    }

    public static readonly DependencyProperty CurrentRiskRangeTextProperty =
        DependencyProperty.Register("CurrentRiskRangeText", typeof(string), typeof(AdminMainWindow), new PropertyMetadata("(N/A)"));

    private string FormatRiskRange(TimeSpan span) =>
        span.TotalHours >= 1 ? $"{(int)span.TotalHours}h {span.Minutes}m" : $"{span.Minutes}m";

    private void BtnHandleVolunteers_Click(object sender, RoutedEventArgs e)
    {
        var window = new VolunteersListWindow(_currentUser);
        window.Closed += (_, _) => _childWindows.Remove(window);
        _childWindows.Add(window);
        window.Show();
    }

    private void BtnHandleCalls_Click(object sender, RoutedEventArgs e)
    {
        var window = new Call.CallsListWindow(_currentUser);
        window.Closed += (_, _) => _childWindows.Remove(window);
        _childWindows.Add(window);
        window.Show();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        if (IsSimulatorRunning)
        {
            try { s_bl.Admin.StopSimulator(); } catch { }
        }

        try { s_bl.Admin.RemoveClockObserver(OnClockUpdated); } catch { }

        foreach (var window in _childWindows.ToArray())
        {
            try { window.Close(); } catch { }
        }

        _childWindows.Clear();
        this.Close();
    }

    private void BtnToggleSimulator_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!IsSimulatorRunning)
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
            }
            else
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error starting/stopping simulator: " + ex.Message);
        }
    }

    private void UpdateStatusCounts()
    {
        if (_statusUpdateOperation is null || _statusUpdateOperation.Status == DispatcherOperationStatus.Completed)
        {
            _statusUpdateOperation = Dispatcher.BeginInvoke(new Action(() =>
            {
                var counts = s_bl.Call.GetCallStatusCounts();
                int safe(int i) => i < counts.Length ? counts[i] : 0;
                int newCount = safe((int)CallStatus.Open) + safe((int)CallStatus.OpenAtRisk);
                int activeCount = safe((int)CallStatus.InTreatment) + safe((int)CallStatus.InTreatmentAtRisk);
                int closedCount = safe((int)CallStatus.Closed) + safe((int)CallStatus.Expired);
                int total = newCount + activeCount + closedCount;

                AllButton.Content = $"All {total}";
                NewButton.Content = $"New {newCount}";
                ActiveButton.Content = $"Active {activeCount}";
                ClosedButton.Content = $"Closed {closedCount}";
            }));
        }
    }

    private void ResetSystemClock(object sender, RoutedEventArgs e)
    {
    }

    protected override void OnClosed(EventArgs e)
    {
        try { s_bl.Admin.RemoveClockObserver(OnClockUpdated); } catch { }

        if (IsSimulatorRunning)
        {
            try { s_bl.Admin.StopSimulator(); } catch { }
        }

        foreach (var window in _childWindows.ToArray())
        {
            try { window.Close(); } catch { }
        }

        _childWindows.Clear();
        base.OnClosed(e);
    }

    private void AllButton_Click(object sender, RoutedEventArgs e)
    {
    }
}