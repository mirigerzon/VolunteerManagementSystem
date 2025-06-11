using System;
using System.ComponentModel;
using System.Windows;
using BO;

namespace PL.Call;

public partial class CallDetailsWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    public Array CallTypes => Enum.GetValues(typeof(CallType));
    public string ButtonText { get; }
    private BO.Call _call;
    public BO.Call Call
    {
        get => _call;
        set
        {
            _call = value;
            OnPropertyChanged(nameof(Call));
        }
    }
    public CallDetailsWindow(BO.Call call)
    {
        InitializeComponent();
        Call = call;
        ButtonText = call.Id == 0 ? "Add" : "Update";
        DataContext = this;
    }
    private void BtnAddOrUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Call.Id == 0)
            {
                s_bl.Call.Create(Call);
                MessageBox.Show("Call added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                s_bl.Call.Update(Call);
                MessageBox.Show("Call updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
