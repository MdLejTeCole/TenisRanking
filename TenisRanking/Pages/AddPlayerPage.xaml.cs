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
public sealed partial class AddPlayerPage : ExtendedPage
{
    public AddPlayerPage()
    {
        this.InitializeComponent();
        Nick.Text = "#" + (new Random()).Next(10000, 99999).ToString();
    }

    private void AddPlayer(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(FirstName.Text) ||  string.IsNullOrEmpty(LastName.Text) || string.IsNullOrEmpty(Nick.Text))
        {
            ShowInfoBar(MissingValuesInfoBar);
            return;
        }
        try
        {
            if (DbContext.Players.Any(x => x.Nick == Nick.Text))
            {
                ShowInfoBar(NotUniqueInfoBar);
                return;
            }
            var elo = DbContext.Settings.First().StartElo;
            var doublElo = DbContext.Settings.First().StartMixedDoubleElo;
            DbContext.Players.Add(new Player() { FirstName = FirstName.Text, LastName = LastName.Text, Nick = Nick.Text, Elo = elo, MixedDoubleElo = doublElo });
            DbContext.SaveChanges();
            ShowInfoBar(SuccessInfoBar);
            FirstName.Text = string.Empty;
            LastName.Text = string.Empty;
            Nick.Text = "#" + (new Random()).Next(10000, 99999).ToString();
        }
        catch (Exception)
        {
            ShowInfoBar(FailedInfoBar);
        }
    }
}
