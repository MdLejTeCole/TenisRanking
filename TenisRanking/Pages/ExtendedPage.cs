using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenisRankingDatabase;
using TenisRankingDatabase.Tables;
using Microsoft.UI.Dispatching;

namespace GameTools.Pages;

public class ExtendedPage : Page, INotifyPropertyChanged
{
    public TenisRankingDbContext DbContext { get; private set; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is TenisRankingDbContext dbContext)
        {
            DbContext = dbContext;
            GetValuesFromDatabase();
        }
    }

    protected virtual void GetValuesFromDatabase()
    {

    }

    protected void ShowInfoBar(InfoBar infoBar)
    {
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _ = Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                infoBar.IsOpen = true;
                await Task.Delay(1000);
                infoBar.IsOpen = false;
            });
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
