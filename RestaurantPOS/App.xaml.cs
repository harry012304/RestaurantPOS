using System.Windows;
using RestaurantPOS.Data;

namespace RestaurantPOS
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            await DatabaseSeeder.SeedUsersAsync();

            base.OnStartup(e);
        }
    }
}