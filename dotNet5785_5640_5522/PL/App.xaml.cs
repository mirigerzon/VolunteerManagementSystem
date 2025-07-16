using System.Configuration;
using System.Data;
using System.Windows;

namespace PL;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SetBrowserEmulationMode(); // קרא לפונקציה הזו פעם אחת באתחול
    }

    private void SetBrowserEmulationMode()
    {
        try
        {
            string exeName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Microsoft.Win32.Registry.SetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                exeName,
                11001, // ערך עבור IE11
                Microsoft.Win32.RegistryValueKind.DWord);
        }
        catch
        {
            // התעלם משגיאות, לא חובה
        }
    }

}
