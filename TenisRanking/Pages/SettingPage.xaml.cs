using Microsoft.UI.Xaml;
using System;
using System.Linq;
using TenisRankingDatabase.Tables;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingPage : ExtendedPage
{
    private Setting _setting;

    public Setting Setting
    {
        get { return _setting; }
        set
        {
            if (_setting != value)
            {
                _setting = value;
                OnPropertyChanged(nameof(Setting));
            }
        }
    }

    public SettingPage()
    {
        this.InitializeComponent();
    }

    protected override void GetValuesFromDatabase()
    {
        Setting = DbContext.Settings.First();
    }

    private void SaveSettings(object sender, RoutedEventArgs e)
    {
        try
        {
            DbContext.Settings.Update(_setting);
            DbContext.SaveChanges();
            ShowInfoBar(SuccessInfoBar);
        }
        catch (Exception)
        {
            ShowInfoBar(FailedInfoBar);
        }
    }
}
