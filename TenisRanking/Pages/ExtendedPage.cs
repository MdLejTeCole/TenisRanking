using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TenisRankingDatabase;

namespace GameTools.Pages;

public class ExtendedPage : Page, INotifyPropertyChanged
{
    public TenisRankingDbContext DbContext { get; private set; }
    public event PropertyChangedEventHandler PropertyChanged;

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

    protected async Task<ContentDialogResult> ShowInformationDialog(string content)
    {
        ContentDialog confirmationDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Information",
            Content = content,
            PrimaryButtonText = "Ok",
        };

        return await confirmationDialog.ShowAsync();
    }

    protected async Task<ContentDialogResult> ShowConfirmationDialog(string content)
    {
        ContentDialog confirmationDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Potwierdzenie",
            Content = content,
            PrimaryButtonText = "Tak",
            CloseButtonText = "Anuluj"
        };

        return await confirmationDialog.ShowAsync();
    }

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
