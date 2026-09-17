using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Windows;
using System.Windows.Controls;

namespace RestaurantPOS.Views
{
    public partial class SalesView : UserControl
    {
        public SalesView()
        {
            InitializeComponent();

            Loaded += SalesView_Loaded;
        }

        private async void SalesView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadSalesAsync();
        }

        private async Task LoadSalesAsync()
        {
            using var context =
                new AppDbContext();

            var orders =
                await context.Orders
                    .Include(o => o.RestaurantTable)
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

            OrdersDataGrid.ItemsSource =
                orders;
        }

        private void OrdersDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem
                is not Order order)
            {
                OrderItemsDataGrid.ItemsSource =
                    null;

                return;
            }

            OrderDetailsTitle.Text =
                $"Order #{order.Id} Details";

            OrderTotalTextBlock.Text =
                $"RM {order.Total:N2}";

            OrderItemsDataGrid.ItemsSource =
                order.OrderItems;
        }
    }
}