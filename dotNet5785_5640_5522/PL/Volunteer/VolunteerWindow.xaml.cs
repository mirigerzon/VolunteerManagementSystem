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

            // רישום משקיף רק במצב עדכון (Id != 0)
            if (Volunteer.Id != 0)
                s_bl.Volunteer.AddObserver(Volunteer.Id, VolunteerObserver);

            // בעת סגירת החלון – הסרת המשקיף
            this.Closed += (s, e) =>
            {
                if (Volunteer.Id != 0)
                    s_bl.Volunteer.RemoveObserver(Volunteer.Id, VolunteerObserver);
            };
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
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.Create(Volunteer);
                    MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Update
                {
                    s_bl.Volunteer.Update(Volunteer.Id, Volunteer);
                    MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // מתודת משקיף שמרעננת את הפריט
        private void VolunteerObserver()
        {
            if (Volunteer?.Id != 0)
            {
                int id = Volunteer.Id;
                Volunteer = null;
                Volunteer = s_bl.Volunteer.Read(id);
            }
        }
    }
}
