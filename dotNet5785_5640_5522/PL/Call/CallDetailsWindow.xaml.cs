using System;
using System.ComponentModel;
using System.Windows;
using System.Linq;

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
            IsNewCall = (_call.Id == 0); 
            UpdateTexts();
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
        Call = call;
        DataContext = this;
    }
    public CallDetailsWindow()
    {
        InitializeComponent();
        Call = new BO.Call
        {
            Id = 0,
            Status = BO.CallStatus.Open, 
            StartTime = DateTime.Now,
            MaxEndTime = DateTime.Now.AddDays(1),
        };
        DataContext = this;
    }
    private void BtnAddOrUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IsNewCall)
            {
                s_bl.Call.Create(Call);
                MessageBox.Show("קריאה חדשה נוספה בהצלחה!", "הוספה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                s_bl.Call.Update(Call);
                MessageBox.Show("קריאה עודכנה בהצלחה!", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.DialogResult = true;
            this.Close();
        }
        catch (BO.BlAlreadyExistsException ex)
        {
            MessageBox.Show($"שגיאה: הקריאה כבר קיימת. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlDoesNotExistException ex)
        {
            MessageBox.Show($"שגיאה: הקריאה לא נמצאה לעדכון. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlInvalidException ex)
        {
            MessageBox.Show($"שגיאה בקלט: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlMissingDataException ex)
        {
            MessageBox.Show($"שגיאה: חסר מידע הכרחי. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (BO.BlDependencyNotInitializedException ex)
        {
            MessageBox.Show($"שגיאה פנימית: תלות לא מאותחלת. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlXMLFileLoadCreateException ex)
        {
            MessageBox.Show($"שגיאה בקובץ XML: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlNullPropertyException ex)
        {
            MessageBox.Show($"שגיאה פנימית: תכונה ריקה. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlInvalidDateRangeException ex)
        {
            MessageBox.Show($"שגיאה: טווח תאריכים שגוי. {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlGeneralException ex)
        {
            MessageBox.Show($"שגיאה כללית: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"אירעה שגיאה בלתי צפויה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}