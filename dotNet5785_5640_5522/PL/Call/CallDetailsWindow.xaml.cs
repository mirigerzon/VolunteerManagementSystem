using System;
using System.ComponentModel;
using System.Windows;
using System.Linq; // For Enum.GetValues

using BO;
using BlApi;
using System.Collections.Generic;

namespace PL.Call;

public partial class CallDetailsWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    public Array CallTypes => Enum.GetValues(typeof(CallType));
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
    private string _headerText;
    public string HeaderText
    {
        get => _headerText;
        set
        {
            _headerText = value;
            OnPropertyChanged(nameof(HeaderText));
        }
    }
    private bool _isNewCall;
    public bool IsNewCall
    {
        get => _isNewCall;
        set
        {
            _isNewCall = value;
            OnPropertyChanged(nameof(IsNewCall));
            // כש-IsNewCall משתנה, נעדכן גם את הטקסטים
            UpdateTexts();
        }
    }
    private BO.Call _call;
    public BO.Call Call
    {
        get => _call;
        set
        {
            _call = value;
            OnPropertyChanged(nameof(Call));
            // גם כשאובייקט ה-Call משתנה, חשוב לעדכן את IsNewCall ואת הטקסטים
            IsNewCall = (_call.Id == 0); // קובע אם זה חדש לפי ID
            UpdateTexts(); // קורא לעדכון טקסטים
        }
    }
    private void UpdateTexts()
    {
        HeaderText = IsNewCall ? "הוספת קריאה חדשה" : "פרטי קריאה";
        ButtonText = IsNewCall ? "הוסף קריאה" : "עדכן קריאה";
    }
    public CallDetailsWindow(BO.Call call)
    {
        InitializeComponent();
        Call = call; // זה יפעיל את ה-setter של Call ויעדכן את IsNewCall והטקסטים
        DataContext = this;
    }
    public CallDetailsWindow()
    {
        InitializeComponent();
        Call = new BO.Call
        {
            Id = 0, // ID 0 מסמן קריאה חדשה
            Status = BO.CallStatus.Open, // סטטוס התחלתי
            StartTime = DateTime.Now,
            MaxEndTime = DateTime.Now.AddDays(1), // זמן סיום מקסימלי ברירת מחדל
        };
        // ה-setter של Call כבר יגדיר את IsNewCall ו-UpdateTexts()
        DataContext = this;
    }
    private void BtnAddOrUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IsNewCall) // נבדוק את IsNewCall במקום Call.Id == 0
            {
                s_bl.Call.Create(Call);
                MessageBox.Show("קריאה חדשה נוספה בהצלחה!", "הוספה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                s_bl.Call.Update(Call);
                MessageBox.Show("קריאה עודכנה בהצלחה!", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.DialogResult = true; // סימן להצלחה לחלון האב
            this.Close();
        }
        catch (BlApi.BlAlreadyExistsException ex) // שימוש ב-BlApi.BlAlreadyExistsException
        {
            MessageBox.Show($"שגיאה: הקריאה כבר קיימת. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BlApi ex) // שימוש ב-BlApi.BlInvalidInputException
        {
            MessageBox.Show($"שגיאה בקלט: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BlApi.BlDoesNotExistException ex) // שימוש ב-BlApi.BlDoesNotExistException
        {
            MessageBox.Show($"שגיאה: הקריאה לא נמצאה לעדכון. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"אירעה שגיאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}