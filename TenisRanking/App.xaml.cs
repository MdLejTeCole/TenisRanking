using GameTools.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using TenisRankingDatabase;
using TenisRankingDatabase.Seeders;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TenisRanking
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Host.CreateDefaultBuilder().
            ConfigureServices((context, services) =>
            {
                services.AddDbContext<TenisRankingDbContext>(options =>
                    options.UseSqlite("Data Source=C:\\Users\\MdLejTeCole\\Desktop\\Db\\TenisRanking.db"));
                services.AddTransient<MainWindow>();
                services.AddScoped<SettingsSeeder>();
                services.AddScoped<TenisRankingSeeder>();
                var serviceProvider = services.BuildServiceProvider();
                var seeder = serviceProvider.GetRequiredService<TenisRankingSeeder>();
                seeder.Seed();
                var dbContext = serviceProvider.GetRequiredService<TenisRankingDbContext>();
                var mainWindow = new MainWindow(dbContext);
                mainWindow.Activate();
            }).Build();
        }
    }
}
