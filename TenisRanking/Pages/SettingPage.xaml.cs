using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;
using TenisRankingDatabase.Tables;
using TenisRankingDatabase;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages
{
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
}
