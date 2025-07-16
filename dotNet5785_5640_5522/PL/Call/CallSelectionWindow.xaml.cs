using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Reflection;

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
                UpdateMap();
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
            UpdateMap();
        }
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void CallsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedCall));
            OnPropertyChanged(nameof(Description));
            UpdateMap();
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
        private volatile bool _isMapLoading = false;
        private string? _pendingHtmlContent = null;
        private async void UpdateMap()
        {
            if (SelectedCall != null && !string.IsNullOrEmpty(SelectedCall.Address))
            {
                try
                {
                    var coords = await GetCoordinatesFromAddressAsync(SelectedCall.Address);
                    if (coords != null)
                    {
                        double lat = coords.Value.lat;
                        double lon = coords.Value.lon;

                        // חישוב zoom level ו-tile coordinates
                        int zoom = 16;
                        int tileX = (int)Math.Floor((lon + 180.0) / 360.0 * Math.Pow(2.0, zoom));
                        int tileY = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2.0, zoom));

                        string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'/>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'/>
    <style>
        html, body {{ 
            margin: 0; 
            padding: 0; 
            height: 100%; 
            font-family: Arial, sans-serif; 
            background: #f0f0f0;
        }}
        .map-container {{ 
            width: 100%; 
            height: 100%; 
            position: relative; 
            overflow: hidden;
            background: #e8e8e8;
        }}
        .map-tile {{ 
            position: absolute; 
            width: 256px; 
            height: 256px; 
        }}
        .marker {{ 
            position: absolute; 
            width: 20px; 
            height: 20px; 
            background: #ff4444; 
            border-radius: 50%; 
            border: 3px solid white; 
            box-shadow: 0 0 10px rgba(0,0,0,0.5);
            z-index: 100;
        }}
        .info-box {{ 
            position: absolute; 
            top: 10px; 
            right: 10px; 
            background: rgba(255,255,255,0.95); 
            padding: 15px; 
            border-radius: 8px; 
            box-shadow: 0 2px 10px rgba(0,0,0,0.2);
            max-width: 250px;
            z-index: 200;
            direction: rtl;
            text-align: right;
        }}
        .info-title {{ 
            font-weight: bold; 
            color: #333; 
            margin-bottom: 8px;
            font-size: 14px;
        }}
        .info-address {{ 
            color: #666; 
            margin-bottom: 5px;
            font-size: 12px;
        }}
        .info-coords {{ 
            color: #999; 
            font-size: 10px;
        }}
        .center-marker {{ 
            position: absolute; 
            top: 50%; 
            left: 50%; 
            transform: translate(-50%, -50%);
        }}
    </style>
</head>
<body>
    <div class='map-container'>
        <!-- רקע המפה -->
        <div style='position: absolute; width: 100%; height: 100%; background: #e8e8e8;'>
            <div style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center; color: #999;'>
                <div style='font-size: 24px; margin-bottom: 10px;'>📍</div>
                <div>מפה סטטית</div>
            </div>
        </div>
        
        <!-- סמן המיקום -->
        <div class='marker center-marker'></div>
        
        <!-- תיבת מידע -->
        <div class='info-box'>
            <div class='info-title'>מיקום הקריאה</div>
            <div class='info-address'>{SelectedCall.Address}</div>
            <div class='info-coords'>
                קואורדינטות: {lat:F6}, {lon:F6}
            </div>
            <div style='margin-top: 10px; padding-top: 10px; border-top: 1px solid #eee; font-size: 11px; color: #888;'>
                זום: {zoom} | רכיב: {tileX}, {tileY}
            </div>
        </div>
        
        <!-- רדיוס -->
        <div style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); 
                    width: 100px; height: 100px; border: 2px solid #ff4444; 
                    border-radius: 50%; opacity: 0.3; z-index: 50;'></div>
    </div>
</body>
</html>";

                        MapWebView.NavigateToString(htmlContent);
                        MapWebView.Visibility = Visibility.Visible;
                        MapPlaceholder.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ShowMapError("לא נמצאה כתובת.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Map Error: {ex.Message}");
                    ShowMapError("שגיאה בטעינת המפה.");
                }
            }
            else
            {
                ShowMapError("בחר קריאה כדי לראות מיקום על גבי מפה.");
            }
        }
        private void ShowMapError(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MapWebView.Visibility = Visibility.Collapsed;
                MapPlaceholder.Visibility = Visibility.Visible;
                MapPlaceholder.Text = message;
            });
        }
        public static async Task<(double lat, double lon)?> GetCoordinatesFromAddressAsync(string address)
        {
            try
            {
                using var httpClient = new HttpClient();
                var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}";
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("VolunteerApp/1.0 (example@email.com)");

                var response = await httpClient.GetStringAsync(url);
                var results = JsonSerializer.Deserialize<List<LocationResult>>(response);
                if (results != null && results.Count > 0)
                {
                    return (double.Parse(results[0].lat, System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(results[0].lon, System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            }

            return null;
        }
        private class LocationResult
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }
    }
}