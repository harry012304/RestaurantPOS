using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Windows;
using System.Windows.Controls;

namespace RestaurantPOS.Views
{
    public partial class MenuView : UserControl
    {
        private int? _selectedProductId;

        public MenuView()
        {
            InitializeComponent();

            Loaded += MenuView_Loaded;
        }

        private async void MenuView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();

            ClearForm();
        }

        private async Task LoadProductsAsync()
        {
            using var context = new AppDbContext();

            var products = await context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ProductsDataGrid.ItemsSource = products;
        }

        private async Task LoadCategoriesAsync()
        {
            using var context = new AppDbContext();

            var categories = await context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            CategoryComboBox.ItemsSource = categories;
        }

        private void ProductsDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is not Product product)
            {
                return;
            }

            _selectedProductId = product.Id;

            FormTitleTextBlock.Text = "Edit Menu Item";

            NameTextBox.Text = product.Name;

            PriceTextBox.Text =
                product.Price.ToString("0.00");

            StockTextBox.Text =
                product.StockQuantity.ToString();

            CategoryComboBox.SelectedValuePath = "Id";

            CategoryComboBox.SelectedValue =
                product.CategoryId;

            DeactivateButton.Visibility =
                Visibility.Visible;

            DeactivateButton.Content =
                product.IsAvailable
                    ? "Deactivate"
                    : "Reactivate";
        }

        private void AddNewButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ProductsDataGrid.SelectedItem = null;

            ClearForm();

            FormTitleTextBlock.Text =
                "Add New Menu Item";

            DeactivateButton.Visibility =
                Visibility.Collapsed;
        }

        private void ClearForm()
        {
            _selectedProductId = null;

            NameTextBox.Text = string.Empty;

            PriceTextBox.Text = string.Empty;

            StockTextBox.Text = string.Empty;

            CategoryComboBox.SelectedItem = null;

            ErrorTextBlock.Text = string.Empty;

            FormTitleTextBlock.Text =
                "Add New Menu Item";

            DeactivateButton.Visibility =
                Visibility.Collapsed;
        }

        private async void SaveButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ErrorTextBlock.Text = string.Empty;

            string name =
                NameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorTextBlock.Text =
                    "Please enter a product name.";

                return;
            }

            if (CategoryComboBox.SelectedItem
                is not Category selectedCategory)
            {
                ErrorTextBlock.Text =
                    "Please select a category.";

                return;
            }

            if (!decimal.TryParse(
                PriceTextBox.Text,
                out decimal price) ||
                price <= 0)
            {
                ErrorTextBlock.Text =
                    "Please enter a valid price.";

                return;
            }

            if (!int.TryParse(
                StockTextBox.Text,
                out int stock) ||
                stock < 0)
            {
                ErrorTextBlock.Text =
                    "Please enter a valid stock quantity.";

                return;
            }

            using var context =
                new AppDbContext();

            if (_selectedProductId == null)
            {
                var product = new Product
                {
                    Name = name,
                    Price = price,
                    StockQuantity = stock,
                    CategoryId = selectedCategory.Id,
                    IsAvailable = true
                };

                context.Products.Add(product);
            }
            else
            {
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

                product.Name = name;
                product.Price = price;
                product.StockQuantity = stock;
                product.CategoryId =
                    selectedCategory.Id;
            }

            await context.SaveChangesAsync();

            await LoadProductsAsync();

            ClearForm();

            MessageBox.Show(
                "Menu item saved successfully.",
                "RestaurantPOS",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private async void DeactivateButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_selectedProductId == null)
            {
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
                return;
            }

            product.IsAvailable =
                !product.IsAvailable;

            await context.SaveChangesAsync();

            await LoadProductsAsync();

            ClearForm();
        }
    }
}