using System.Windows;
using System.Windows.Controls;
using RestaurantPOS.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using System.Windows.Media;

namespace RestaurantPOS.Views
{
    public partial class DashboardView : UserControl
    {
        //recent sale
        public class DashboardRecentSale
        {
            public string OrderNumber { get; set; }
                = string.Empty;

            public string OrderType { get; set; }
                = string.Empty;

            public string TableNumber { get; set; }
                = string.Empty;

            public decimal Total { get; set; }

            public DateTime OrderDate { get; set; }
        }

        public class DashboardPopularItem
        {
            public string ProductName { get; set; }
                = string.Empty;

            public int QuantitySold { get; set; }
        }

        private readonly User _currentUser;

        public DashboardView(User user)
        {
            InitializeComponent();

            _currentUser = user;


            WelcomeTextBlock.Text =
                $"Welcome back, {user.FullName}!";

            UserNameTextBlock.Text =
                user.FullName;

            RoleTextBlock.Text =
                user.Role;

            ApplyRolePermissions();

            SetActiveButton(DashboardButton);

            Loaded += DashboardView_Loaded;
        }

        //set active btn
        private void SetActiveButton(Button activeButton)
        {
            var buttons = new[]
            {
                DashboardButton,
                POSButton,
                TablesButton,
                MenuButton,
                InventoryButton,
                SalesButton
            };

            foreach (var button in buttons)
            {
                button.Background =
                    Brushes.Transparent;

                button.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(209, 213, 219));
            }

            activeButton.Background =
                new SolidColorBrush(
                    Color.FromRgb(37, 99, 235));

            activeButton.Foreground =
                Brushes.White;
        }

        //role permission
        private void ApplyRolePermissions()
        {
            bool isAdmin =
                _currentUser.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);

            MenuButton.Visibility =
                isAdmin
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            InventoryButton.Visibility =
                isAdmin
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        //dashboard data
        private async void DashboardView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadDashboardStatsAsync();
        }

        //load dashboard
        private async Task LoadDashboardStatsAsync()
        {
            using var context =
                new AppDbContext();

            DateTime today =
                DateTime.Today;

            DateTime tomorrow =
                today.AddDays(1);

            // =========================
            // TODAY'S ORDERS
            // =========================

            var todayOrders =
                await context.Orders
                    .Where(o =>
                        o.OrderDate >= today &&
                        o.OrderDate < tomorrow &&
                        o.Status == "Completed")
                    .ToListAsync();

            decimal todaySales =
                todayOrders.Sum(o => o.Total);

            int ordersCount =
                todayOrders.Count;

            int productsCount =
                await context.Products
                    .CountAsync();

            int lowStockCount =
            await context.Products
            .CountAsync(p =>
            p.StockQuantity > 0 &&
            p.StockQuantity <= 5);

            TodaySalesTextBlock.Text =
                $"RM {todaySales:N2}";

            OrdersCountTextBlock.Text =
                ordersCount.ToString();

            ProductsCountTextBlock.Text =
                productsCount.ToString();

            LowStockCountTextBlock.Text =
                lowStockCount.ToString();

            // =========================
            // RECENT SALES
            // =========================

            var recentOrders =
                await context.Orders
                    .Include(o => o.RestaurantTable)
                    .Where(o =>
                        o.Status == "Completed")
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync();

            var recentSales =
                recentOrders
                    .Select(o =>
                        new DashboardRecentSale
                        {
                            OrderNumber =
                                $"#{o.Id}",

                            OrderType =
                                o.OrderType,

                            TableNumber =
                                o.RestaurantTable?.TableNumber
                                ?? "-",

                            Total =
                                o.Total,

                            OrderDate =
                                o.OrderDate
                        })
                    .ToList();

            RecentSalesDataGrid.ItemsSource =
                recentSales;

            // =========================
            // POPULAR PRODUCTS
            // =========================

            var soldItems =
                await context.OrderItems
                    .Include(i => i.Order)
                    .Where(i =>
                        i.Order != null &&
                        i.Order.Status == "Completed")
                    .ToListAsync();

            var popularItems =
                soldItems
                    .GroupBy(i => i.ProductName)
                    .Select(group =>
                        new DashboardPopularItem
                        {
                            ProductName =
                                group.Key,

                            QuantitySold =
                                group.Sum(i =>
                                    i.Quantity)
                        })
                    .OrderByDescending(i =>
                        i.QuantitySold)
                    .Take(5)
                    .ToList();

            PopularItemsControl.ItemsSource =
                popularItems;
        }

        //POS
        private void POSButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DashboardContent.Visibility =
                Visibility.Collapsed;

            PageContent.Content =
                new POSView();

            PageContent.Visibility =
                Visibility.Visible;

            SetActiveButton(POSButton);
        }

        //table button
        private void TablesButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DashboardContent.Visibility = Visibility.Collapsed;

            PageContent.Content = new TablesView();

            PageContent.Visibility = Visibility.Visible;

            SetActiveButton(TablesButton);
        }

        //dashboard button
        private async void DashboardButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            PageContent.Visibility =
                Visibility.Collapsed;

            PageContent.Content =
                null;

            DashboardContent.Visibility =
                Visibility.Visible;

            await LoadDashboardStatsAsync();

            SetActiveButton(DashboardButton);
        }

        //menu button
        private void MenuButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!_currentUser.Role.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "You do not have permission to access Menu Management.",
                    "Access Denied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DashboardContent.Visibility =
                Visibility.Collapsed;

            PageContent.Content =
                new MenuView();

            PageContent.Visibility =
                Visibility.Visible;

            SetActiveButton(MenuButton);
        }

        //invetory button
        private void InventoryButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!_currentUser.Role.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "You do not have permission to access Inventory Management.",
                    "Access Denied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DashboardContent.Visibility =
                Visibility.Collapsed;

            PageContent.Content =
                new InventoryView();

            PageContent.Visibility =
                Visibility.Visible;

            SetActiveButton(InventoryButton);
        }

        //sales button
        private void SalesButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DashboardContent.Visibility =
                Visibility.Collapsed;

            PageContent.Content =
                new SalesView();

            PageContent.Visibility =
                Visibility.Visible;

            SetActiveButton(SalesButton);
        }

        //logout
        private void LogoutButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content =
                new LoginView();
        }
    }
}