using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RestaurantPOS.Views
{
    public partial class TablesView : UserControl
    {
        public TablesView()
        {
            InitializeComponent();

            Loaded += TablesView_Loaded;
        }

        private async void TablesView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadTablesAsync();
        }

        private async Task LoadTablesAsync()
        {
            using var context = new AppDbContext();

            var tables = await context.RestaurantTables
                .OrderBy(t => t.Id)
                .ToListAsync();

            TablesItemsControl.ItemsSource = tables;
        }

        private async void TableCard_Click(
            object sender,
            MouseButtonEventArgs e)
        {
            if (sender is not Border border ||
                border.DataContext is not RestaurantTable selectedTable)
            {
                return;
            }

            using var context = new AppDbContext();

            var table = await context.RestaurantTables
                .FirstOrDefaultAsync(t => t.Id == selectedTable.Id);

            if (table == null)
            {
                return;
            }

            table.Status = table.Status switch
            {
                "Available" => "Occupied",
                "Occupied" => "Reserved",
                "Reserved" => "Available",
                _ => "Available"
            };

            await context.SaveChangesAsync();

            await LoadTablesAsync();
        }
    }
}