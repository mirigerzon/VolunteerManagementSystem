using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Xml.Linq;

using System;

using System;
using System.ComponentModel;
using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteerDetailsWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public VolunteerDetailsWindow(BO.Volunteer volunteer)
        {
            Volunteer = volunteer;
            ButtonText = volunteer.Id == 0 ? "Add" : "Update";
            InitializeComponent();
            DataContext = this;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        private BO.Volunteer _volunteer;
        public BO.Volunteer Volunteer
        {
            get => _volunteer;
            set
            {
                _volunteer = value;
                OnPropertyChanged(nameof(Volunteer));
            }
        }
        private string _buttonText;
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer.Id == 0)
                    s_bl.Volunteer.Create(Volunteer);
                else
                    s_bl.Volunteer.Update(Volunteer.Id, Volunteer);

                MessageBox.Show($"{ButtonText} succeeded", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}