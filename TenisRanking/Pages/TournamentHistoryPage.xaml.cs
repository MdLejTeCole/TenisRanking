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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TournamentHistoryPage : ExtendedPage
    {
        public TournamentHistoryPage()
        {
            this.InitializeComponent();
        }

        protected override void GetValuesFromDatabase()
        {
            MyDataGrid.ItemsSource = null;
            var tournamentsHistory = DbContext.Tournaments
                .Include(x => x.TournamentPlayers)
                    .ThenInclude(x => x.Player)
                .Where(x => x.Ended)
                .OrderByDescending(x => x.Id)
                .Select(x => TournamentHistory.Create(x));
            MyDataGrid.ItemsSource = tournamentsHistory;
        }
    }
}