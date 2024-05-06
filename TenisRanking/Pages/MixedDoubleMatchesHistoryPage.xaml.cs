using CommunityToolkit.WinUI.UI.Controls;
using GameTools.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TenisRankingDatabase.Enums;
using TenisRankingDatabase.Tables;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MixedDoubleMatchesHistoryPage : ExtendedPage
    {
        public MixedDoubleMatchesHistoryPage()
        {
            this.InitializeComponent();
        }

        protected override void GetValuesFromDatabase()
        {
            MyDataGrid.ItemsSource = null;
            var matchesHistory = DbContext.Matches
                .Include(x => x.Tournament)
                .Include(x => x.PlayerMatches)
                    .ThenInclude(x => x.Player)
                .Where(x => x.Tournament.TenisMatchType == TenisMatchType.MixedDouble)
                .OrderByDescending(x => x.Id)
                .Select(x => DoubleMatchHistory.Create(x));
            MyDataGrid.ItemsSource = matchesHistory;
        }
    }
}
