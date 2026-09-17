using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using RestaurantPOS.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using RestaurantPOS.Services;

namespace RestaurantPOS.Views
{
    public partial class POSView : UserControl
    {
        private readonly ObservableCollection<CartItem> _cartItems = new();

        private List<Product> _allProducts = new();

        private int? _selectedTableId;

        private bool _isLoadingTables;

        private readonly OrderCalculationService
            _calculationService = new();

        public POSView()
        {
            InitializeComponent();

            CartItemsControl.ItemsSource = _cartItems;

            Loaded += POSView_Loaded;
        }

        private async void POSView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadProductsAsync();

            await LoadAvailableTablesAsync();

            OrderTypeComboBox.SelectedIndex = 0;
        }

        private async Task LoadProductsAsync()
        {
            using var context = new AppDbContext();

            _allProducts = await context.Products
                .Include(p => p.Category)
                .Where(p => p.IsAvailable)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var categories = await context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            categories.Insert(0, new Category
            {
                Id = 0,
                Name = "All"
            });

            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;

            ProductsItemsControl.ItemsSource = _allProducts;
        }

        private void CategoryComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is not Category category)
            {
                return;
            }

            if (category.Id == 0)
            {
                ProductsItemsControl.ItemsSource = _allProducts;
            }
            else
            {
                ProductsItemsControl.ItemsSource =
                    _allProducts
                        .Where(p => p.CategoryId == category.Id)
                        .ToList();
            }
        }

        private void AddProductButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not Product product)
            {
                return;
            }

            var existingItem =
                _cartItems.FirstOrDefault(
                    item => item.ProductId == product.Id);

            if (existingItem != null)
            {
                if (existingItem.Quantity >= product.StockQuantity)
                {
                    MessageBox.Show(
                        "Not enough stock available.",
                        "Stock Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                existingItem.Quantity++;
            }
            else
            {
                if (product.StockQuantity <= 0)
                {
                    MessageBox.Show(
                        "This product is out of stock.",
                        "Stock Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                _cartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = 1
                });
            }

            CalculateTotals();
        }

        //increase btn
        private void IncreaseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not CartItem item)
            {
                return;
            }

            var product = _allProducts
                .FirstOrDefault(p => p.Id == item.ProductId);

            if (product == null)
            {
                return;
            }

            if (item.Quantity >= product.StockQuantity)
            {
                MessageBox.Show(
                    "Not enough stock available.",
                    "Stock Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            item.Quantity++;

            CalculateTotals();
        }

        //dec btn
        private void DecreaseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not CartItem item)
            {
                return;
            }

            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                _cartItems.Remove(item);
            }

            CalculateTotals();
        }

        //calc total
        private void CalculateTotals()
        {
            var totals =
                _calculationService.Calculate(
                    _cartItems);

            SubtotalTextBlock.Text =
                $"RM {totals.Subtotal:N2}";

            TaxTextBlock.Text =
                $"RM {totals.Tax:N2}";

            TotalTextBlock.Text =
                $"RM {totals.Total:N2}";
        }

        //save order
        private async Task<int> SaveOrderAsync(
            string orderType,
            decimal subtotal,
            decimal tax,
            decimal total,
            decimal amountReceived)
        {
            using var context =
                new AppDbContext();

            await using var transaction =
                await context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now,

                    OrderType = orderType,

                    RestaurantTableId =
                        orderType == "Dine-In"
                            ? _selectedTableId
                            : null,

                    Subtotal = subtotal,

                    Tax = tax,

                    Total = total,

                    PaymentMethod = "Cash",

                    AmountReceived = amountReceived,

                    ChangeAmount =
                        amountReceived - total,

                    Status = "Completed"
                };

                foreach (var cartItem in _cartItems)
                {
                    var product =
                        await context.Products
                            .FirstOrDefaultAsync(
                                p => p.Id ==
                                     cartItem.ProductId);

                    if (product == null)
                    {
                        throw new Exception(
                            $"{cartItem.ProductName} no longer exists.");
                    }

                    if (product.StockQuantity <
                        cartItem.Quantity)
                    {
                        throw new Exception(
                            $"Not enough stock for {product.Name}.");
                    }

                    // Deduct stock
                    product.StockQuantity -=
                        cartItem.Quantity;

                    // Save order item
                    order.OrderItems.Add(
                        new OrderItem
                        {
                            ProductId =
                                cartItem.ProductId,

                            ProductName =
                                cartItem.ProductName,

                            UnitPrice =
                                cartItem.UnitPrice,

                            Quantity =
                                cartItem.Quantity,

                            Total =
                                cartItem.Total
                        });
                }

                // Release table after payment
                if (order.RestaurantTableId.HasValue)
                {
                    var table =
                        await context.RestaurantTables
                            .FirstOrDefaultAsync(
                                t => t.Id ==
                                order.RestaurantTableId.Value);

                    if (table != null)
                    {
                        table.Status =
                            "Available";
                    }
                }

                context.Orders.Add(order);

                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return order.Id;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        //clear order
        private async void ClearOrderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            _cartItems.Clear();

            CalculateTotals();

            await ReleaseCurrentTableAsync();

            await LoadAvailableTablesAsync();

            OrderTypeComboBox.SelectedIndex = 0;
        }

        //payment btn
        private async void PaymentButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show(
                    "Please add at least one item.",
                    "Empty Order",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            if (OrderTypeComboBox.SelectedItem
                is not ComboBoxItem selectedItem)
            {
                return;
            }

            string orderType =
                selectedItem.Content?.ToString()
                ?? string.Empty;

            if (orderType == "Dine-In" &&
                _selectedTableId == null)
            {
                MessageBox.Show(
                    "Please select a table for dine-in.",
                    "Table Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var totals =
                _calculationService.Calculate(
                    _cartItems);

            decimal subtotal =
                totals.Subtotal;

            decimal tax =
                totals.Tax;

            decimal total =
                totals.Total;

            var paymentWindow =
                new PaymentWindow(total);

            paymentWindow.Owner =
                Window.GetWindow(this);

            bool? result =
                paymentWindow.ShowDialog();

            if (result != true)
            {
                return;
            }

            try
            {
                int orderId =
                    await SaveOrderAsync(
                        orderType,
                        subtotal,
                        tax,
                        total,
                        paymentWindow.AmountReceived);

                _selectedTableId = null;

                _cartItems.Clear();

                CalculateTotals();

                await LoadProductsAsync();

                await LoadAvailableTablesAsync();

                OrderTypeComboBox.SelectedIndex = 0;

                //receipt
                var receiptWindow = new ReceiptWindow(orderId);

                receiptWindow.Owner = Window.GetWindow(this);

                receiptWindow.ShowDialog();
                //receipt end

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Payment could not be completed.\n\n{ex.Message}",
                    "Payment Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task LoadAvailableTablesAsync()
        {
            _isLoadingTables = true;

            using var context = new AppDbContext();

            var tables = await context.RestaurantTables
                .Where(t => t.Status == "Available")
                .OrderBy(t => t.Id)
                .ToListAsync();

            TableComboBox.ItemsSource = tables;
            TableComboBox.SelectedItem = null;

            _isLoadingTables = false;
        }


        //handle dine-in / takeaway
        private async void OrderTypeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (OrderTypeComboBox.SelectedItem is not ComboBoxItem selectedItem)
            {
                return;
            }

            string orderType =
                selectedItem.Content?.ToString() ?? string.Empty;

            if (orderType == "Dine-In")
            {
                TableSelectionPanel.Visibility =
                    Visibility.Visible;

                await LoadAvailableTablesAsync();
            }
            else
            {
                TableSelectionPanel.Visibility =
                    Visibility.Collapsed;

                await ReleaseCurrentTableAsync();

                TableComboBox.SelectedItem = null;
            }
        }

        //make selected table occupied
        private async void TableComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_isLoadingTables)
            {
                return;
            }

            if (TableComboBox.SelectedItem is not RestaurantTable selectedTable)
            {
                return;
            }

            if (_selectedTableId == selectedTable.Id)
            {
                return;
            }

            await ReleaseCurrentTableAsync();

            using var context = new AppDbContext();

            var table = await context.RestaurantTables
                .FirstOrDefaultAsync(t => t.Id == selectedTable.Id);

            if (table == null)
            {
                return;
            }

            table.Status = "Occupied";

            await context.SaveChangesAsync();

            _selectedTableId = table.Id;
        }


        //table release method
        private async Task ReleaseCurrentTableAsync()
        {
            if (_selectedTableId == null)
            {
                return;
            }

            using var context = new AppDbContext();

            var table = await context.RestaurantTables
                .FirstOrDefaultAsync(
                    t => t.Id == _selectedTableId.Value);

            if (table != null)
            {
                table.Status = "Available";

                await context.SaveChangesAsync();
            }

            _selectedTableId = null;
        }

    }
}