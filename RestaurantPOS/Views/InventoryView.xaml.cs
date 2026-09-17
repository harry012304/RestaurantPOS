using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using System.Windows;
using System.Windows.Controls;

namespace RestaurantPOS.Views
{
    public partial class InventoryView : UserControl
    {
        private int? _selectedProductId;

        private List<InventoryRow> _inventoryItems = new();

        public InventoryView()
        {
            InitializeComponent();

            Loaded += InventoryView_Loaded;
        }

        private async void InventoryView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            using var context =
                new AppDbContext();

            var products =
                await context.Products
                    .Include(p => p.Category)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

            _inventoryItems = products
                .Select(p => new InventoryRow
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category =
                        p.Category?.Name ?? "-",
                    Price = p.Price,
                    StockQuantity =
                        p.StockQuantity,
                    StockStatus =
                        GetStockStatus(
                            p.StockQuantity)
                })
                .ToList();

            InventoryDataGrid.ItemsSource =
                _inventoryItems;

            TotalProductsTextBlock.Text =
                _inventoryItems.Count.ToString();

            LowStockTextBlock.Text =
                _inventoryItems
                    .Count(p =>
                        p.StockQuantity <= 5)
                    .ToString();
        }

        private static string GetStockStatus(
            int stock)
        {
            if (stock == 0)
            {
                return "Out of Stock";
            }

            if (stock <= 5)
            {
                return "Low Stock";
            }

            return "In Stock";
        }

        private void InventoryDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem
                is not InventoryRow item)
            {
                return;
            }

            _selectedProductId =
                item.Id;

            SelectedProductTextBlock.Text =
                item.Name;

            CurrentStockTextBlock.Text =
                item.StockQuantity.ToString();

            RestockQuantityTextBox.Text =
                string.Empty;

            ErrorTextBlock.Text =
                string.Empty;
        }

        private async void RestockButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ErrorTextBlock.Text =
                string.Empty;

            if (_selectedProductId == null)
            {
                ErrorTextBlock.Text =
                    "Please select a product first.";

                return;
            }

            if (!int.TryParse(
                RestockQuantityTextBox.Text,
                out int quantity) ||
                quantity <= 0)
            {
                ErrorTextBlock.Text =
                    "Please enter a valid quantity.";

                return;
            }

            using var context =
                new AppDbContext();

            var product =
                await context.Products
                    .FirstOrDefaultAsync(
                        p => p.Id ==
                        _selectedProductId.Value);

            if (product == null)
            {
                ErrorTextBlock.Text =
                    "Product could not be found.";

                return;
            }

            product.StockQuantity +=
                quantity;

            await context.SaveChangesAsync();

            MessageBox.Show(
                $"{quantity} unit(s) added to {product.Name}.",
                "Stock Updated",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadInventoryAsync();

            SelectedProductTextBlock.Text =
                product.Name;

            CurrentStockTextBlock.Text =
                product.StockQuantity
                    .ToString();

            RestockQuantityTextBox.Text =
                string.Empty;
        }

        private void SearchTextBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            string search =
                SearchTextBox.Text
                    .Trim()
                    .ToLower();

            if (string.IsNullOrWhiteSpace(search))
            {
                InventoryDataGrid.ItemsSource =
                    _inventoryItems;

                return;
            }

            InventoryDataGrid.ItemsSource =
                _inventoryItems
                    .Where(p =>
                        p.Name
                            .ToLower()
                            .Contains(search) ||
                        p.Category
                            .ToLower()
                            .Contains(search))
                    .ToList();
        }
    }

    public class InventoryRow
    {
        public int Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public string Category { get; set; }
            = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string StockStatus { get; set; }
            = string.Empty;
    }
}