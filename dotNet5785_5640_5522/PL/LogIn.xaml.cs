using BlApi;
using PL.Volunteer;
using System.Windows.Controls;
using System.Windows;
using System;
using BO;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PL;
public partial class Login : Window
{
    private static readonly IBl s_bl = Factory.Get();
    private static bool _isAdminCurrentlyLoggedIn = false;

    private void IdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
    }

    public Login()
    {
        InitializeComponent();

        this.Closing += (s, e) =>
        {
            e.Cancel = true;
            this.Hide();
        };
        this.Loaded += (s, e) => OpenLoginWindow();
    }

    private void OpenLoginWindow()
    {
        this.Show();
        this.Activate();
        IdTextBox.Clear();
        PasswordBox.Clear();
        StatusTextBlock.Text = string.Empty;
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        int id = int.Parse(IdTextBox.Text.Trim());
        string password = PasswordBox.Password.Trim();

        try
        {
            BO.UserRole? authenticatedVolunteer = s_bl.Volunteer.Login(id, password);
            if (authenticatedVolunteer != null)
            {
                BO.Volunteer volunteer = s_bl.Volunteer.Read(id); // שורה חדשה: שולף את כל פרטי המשתמש

                if (authenticatedVolunteer == UserRole.Admin)
                {
                    if (_isAdminCurrentlyLoggedIn)
                    {
                        StatusTextBlock.Text = "An administrator is already logged in. Only one admin session is allowed.";
                        return;
                    }

                    _isAdminCurrentlyLoggedIn = true;
                    ShowAdminSelectionScreen(volunteer); // מעביר את האובייקט המלא
                    this.Hide();
                }
                else
                {
                    VolunteersListWindow volunteerListWindow = new VolunteersListWindow(volunteer); // מעביר את המתנדב
                    volunteerListWindow.Closed += (s, args) => OpenLoginWindow();
                    volunteerListWindow.Show();
                    this.Hide();
                }
            }
            else
            {
                StatusTextBlock.Text = "Invalid ID or password. Please try again.";
            }
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = $"Login failed: {ex.Message}";
            MessageBox.Show($"An error occurred during login: {ex.Message}", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ShowAdminSelectionScreen(BO.Volunteer admin)
    {
        Window adminSelectionWindow = new Window();
        adminSelectionWindow.Title = "Admin Login Options";
        adminSelectionWindow.Width = 300;
        adminSelectionWindow.Height = 200;
        adminSelectionWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        adminSelectionWindow.Content = new StackPanel
        {
            Margin = new Thickness(20),
            VerticalAlignment = VerticalAlignment.Center,
            Children =
        {
            new TextBlock
            {
                Text = $"Hello, Admin {admin.Id}!",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0,0,0,20),
                FontWeight = FontWeights.Bold
            },
            new Button { Content = "Go to Volunteer List", Margin = new Thickness(0,5,0,5), Padding = new Thickness(10, 5, 10, 5) }
            .With(b => b.Click += (s, e) =>
            {
                VolunteersListWindow volunteerListWindow = new VolunteersListWindow(admin);
                volunteerListWindow.Closed += (ws, we) => {
                    _isAdminCurrentlyLoggedIn = false;
                    OpenLoginWindow();
                };
                volunteerListWindow.Show();
                adminSelectionWindow.Close();
            }),
            new Button { Content = "Go to Admin Management", Margin = new Thickness(0,5,0,5), Padding = new Thickness(10, 5, 10, 5) }
            .With(b => b.Click += (s, e) =>
            {
                AdminMainWindow adminScreen = new AdminMainWindow(admin);
                adminScreen.Closed += (ws, we) => {
                    _isAdminCurrentlyLoggedIn = false;
                    OpenLoginWindow();
                };
                adminScreen.Show();
                adminSelectionWindow.Close();
            })
        }
        };
        adminSelectionWindow.ShowDialog();
    }
}

public static class ControlExtensions
{
    public static T With<T>(this T control, System.Action<T> action) where T : Control
    {
        action(control);
        return control;
    }
}
