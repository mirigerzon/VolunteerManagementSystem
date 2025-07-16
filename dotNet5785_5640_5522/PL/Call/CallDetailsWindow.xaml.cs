using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using BO;
using BlApi;
using System.Collections.Generic;

namespace PL.Call
{
    public partial class CallDetailsWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Array CallTypes => Enum.GetValues(typeof(CallType));

        // Time picker properties
        public List<int> Hours => Enumerable.Range(0, 24).ToList();
        public List<int> Minutes => Enumerable.Range(0, 60).Where(m => m % 5 == 0).ToList();

        private string _buttonText;
        public string ButtonText
        {
            get => _buttonText;
            set { _buttonText = value; OnPropertyChanged(nameof(ButtonText)); }
        }

        private string _headerText;
        public string HeaderText
        {
            get => _headerText;
            set { _headerText = value; OnPropertyChanged(nameof(HeaderText)); }
        }

        private bool _isNewCall;
        public bool IsNewCall
        {
            get => _isNewCall;
            set { _isNewCall = value; OnPropertyChanged(nameof(IsNewCall)); UpdateTexts(); }
        }

        private BO.Call _call;
        public BO.Call Call
        {
            get => _call;
            set
            {
                _call = value;
                OnPropertyChanged(nameof(Call));
                IsNewCall = (_call.Id == 0);
                UpdateTexts();
                UpdateDateTimeFromCall();
            }
        }

        private DateTime? _maxEndDate;
        public DateTime? MaxEndDate
        {
            get => _maxEndDate;
            set
            {
                _maxEndDate = value;
                OnPropertyChanged(nameof(MaxEndDate));
                UpdateCallMaxEndTime();
            }
        }

        private int _selectedHour;
        public int SelectedHour
        {
            get => _selectedHour;
            set
            {
                _selectedHour = value;
                OnPropertyChanged(nameof(SelectedHour));
                UpdateCallMaxEndTime();
            }
        }

        private int _selectedMinute;
        public int SelectedMinute
        {
            get => _selectedMinute;
            set
            {
                _selectedMinute = value;
                OnPropertyChanged(nameof(SelectedMinute));
                UpdateCallMaxEndTime();
            }
        }

        private string _maxEndTimeDisplay;
        public string MaxEndTimeDisplay
        {
            get => _maxEndTimeDisplay;
            set { _maxEndTimeDisplay = value; OnPropertyChanged(nameof(MaxEndTimeDisplay)); }
        }

        private void UpdateTexts()
        {
            HeaderText = IsNewCall ? "Add New Call" : "Call Details";
            ButtonText = IsNewCall ? "Add Call" : "Update Call";
        }

        private void UpdateDateTimeFromCall()
        {
            if (Call?.MaxEndTime != null)
            {
                MaxEndDate = Call.MaxEndTime.Value.Date;
                SelectedHour = Call.MaxEndTime.Value.Hour;
                SelectedMinute = Call.MaxEndTime.Value.Minute;
                UpdateDisplayText();
            }
        }

        private void UpdateCallMaxEndTime()
        {
            if (Call != null && MaxEndDate.HasValue)
            {
                try
                {
                    var newDateTime = new DateTime(
                        MaxEndDate.Value.Year,
                        MaxEndDate.Value.Month,
                        MaxEndDate.Value.Day,
                        SelectedHour,
                        SelectedMinute,
                        0
                    );
                    Call.MaxEndTime = newDateTime;
                    OnPropertyChanged(nameof(Call));
                    UpdateDisplayText();
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Handle invalid date/time combinations
                }
            }
        }

        private void UpdateDisplayText()
        {
            if (Call?.MaxEndTime != null)
            {
                MaxEndTimeDisplay = Call.MaxEndTime.Value.ToString("dd/MM/yyyy HH:mm");
            }
        }

        private volatile DispatcherOperation? _observerOperation = null;

        public CallDetailsWindow(BO.Call call)
        {
            InitializeComponent();
            Call = call;
            DataContext = this;
        }

        public CallDetailsWindow()
        {
            InitializeComponent();
            var defaultDateTime = DateTime.Now.AddDays(1);
            Call = new BO.Call
            {
                Id = 0,
                Status = BO.CallStatus.Open,
                StartTime = DateTime.Now,
                MaxEndTime = defaultDateTime,
            };

            // Set default time picker values
            MaxEndDate = defaultDateTime.Date;
            SelectedHour = defaultDateTime.Hour;
            SelectedMinute = (defaultDateTime.Minute / 5) * 5; // Round to nearest 5 minutes
            UpdateDisplayText();

            DataContext = this;
        }

        private void DateTimePickerButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = FindName("DateTimePopup") as System.Windows.Controls.Primitives.Popup;
            if (popup != null)
            {
                popup.IsOpen = true;
            }
        }

        private void SetDateTime_Click(object sender, RoutedEventArgs e)
        {
            var popup = FindName("DateTimePopup") as System.Windows.Controls.Primitives.Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
            }
            UpdateCallMaxEndTime();
        }

        private void CancelDateTime_Click(object sender, RoutedEventArgs e)
        {
            var popup = FindName("DateTimePopup") as System.Windows.Controls.Primitives.Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
            }
            // Reset to original values
            UpdateDateTimeFromCall();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(RefreshCall);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(RefreshCall);
        }

        private void RefreshCall()
        {
            if (IsNewCall || Call.Id == 0)
                return;

            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        Call = s_bl.Call.Read(Call.Id);
                    }
                    catch
                    {
                        // If call was deleted, no need to update
                    }
                }));
            }
        }

        private void BtnAddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNewCall)
                {
                    s_bl.Call.Create(Call);
                    MessageBox.Show("New call added successfully!", "Addition", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Call.Update(Call);
                    MessageBox.Show("Call updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                MessageBox.Show($"Error: Call already exists. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show($"Error: Call not found for update. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlInvalidException ex)
            {
                MessageBox.Show($"Input error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlMissingDataException ex)
            {
                MessageBox.Show($"Error: Missing required information. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlDependencyNotInitializedException ex)
            {
                MessageBox.Show($"Internal error: Dependency not initialized. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlXMLFileLoadCreateException ex)
            {
                MessageBox.Show($"XML file error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlNullPropertyException ex)
            {
                MessageBox.Show($"Internal error: Null property. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlInvalidDateRangeException ex)
            {
                MessageBox.Show($"Error: Invalid date range. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlGeneralException ex)
            {
                MessageBox.Show($"General error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}