using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using TenisRankingDatabase;
using TenisRankingDatabase.Tables;

namespace TenisRanking.Pages.MainWindow;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel(TenisRankingDbContext dbContext)
    {
		try
		{
			var a = dbContext.Players.Add(new Player());
			var b = dbContext.Players.ToList();
		}
		catch (Exception e)
		{

			throw;
		}
    }
}
