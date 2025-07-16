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

        private async void UpdateMap()
        {
            if (SelectedCall != null && !string.IsNullOrEmpty(SelectedCall.Address))
            {
                try
                {
                    var coords = await GetCoordinatesFromAddressAsync(SelectedCall.Address);
                    if (coords != null)
                    {
                        string lat = coords.Value.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string lon = coords.Value.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        string htmlContent = $@"
                            <html>
                              <head>
                                <meta http-equiv='X-UA-Compatible' content='IE=Edge'/>
                                <style>
                                  html, body {{ margin: 0; padding: 0; height: 100%; }}
                                  iframe {{ width: 100%; height: 100%; border: none; }}
                                </style>
                              </head>
                              <body>
                                <iframe 
                                  src='https://www.openstreetmap.org/export/embed.html?bbox={lon}%2C{lat}%2C{lon}%2C{lat}&layer=mapnik&marker={lat}%2C{lon}'>
                                </iframe>
                              </body>
                            </html>";
                        MapWebBrowser.NavigateToString(htmlContent);
                        MapWebBrowser.Visibility = Visibility.Visible;
                        MapPlaceholder.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MapWebBrowser.Visibility = Visibility.Collapsed;
                        MapPlaceholder.Visibility = Visibility.Visible;
                        MapPlaceholder.Text = "Could not find location for address";
                    }
                }
                catch
                {
                    MapWebBrowser.Visibility = Visibility.Collapsed;
                    MapPlaceholder.Visibility = Visibility.Visible;
                    MapPlaceholder.Text = "Error loading map";
                }
            }
            else
            {
                MapWebBrowser.Visibility = Visibility.Collapsed;
                MapPlaceholder.Visibility = Visibility.Visible;
                MapPlaceholder.Text = "Select a call to view location on map";
            }
        }

        public async Task<(double Latitude, double Longitude)?> GetCoordinatesFromAddressAsync(string address)
        {
            try
            {
                using var httpClient = new HttpClient();
                string url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}&limit=1";

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName/1.0"); // חובה לפי תנאי Nominatim

                string response = await httpClient.GetStringAsync(url);

                var results = JsonSerializer.Deserialize<List<NominatimResult>>(response);
                if (results != null && results.Count > 0)
                {
                    double lat = double.Parse(results[0].Lat, System.Globalization.CultureInfo.InvariantCulture);
                    double lon = double.Parse(results[0].Lon, System.Globalization.CultureInfo.InvariantCulture);
                    return (lat, lon);
                }
            }
            catch
            {
                // התעלם משגיאה או החזר null
            }

            return null;
        }

        public class NominatimResult
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
        }

    }
}