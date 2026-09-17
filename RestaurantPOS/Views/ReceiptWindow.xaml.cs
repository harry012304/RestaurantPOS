using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Data;
using System.Windows;
using System.Windows.Controls;

namespace RestaurantPOS.Views
{
    public partial class ReceiptWindow : Window
    {
        private readonly int _orderId;

        public ReceiptWindow(int orderId)
        {
            InitializeComponent();

            _orderId = orderId;

            Loaded += ReceiptWindow_Loaded;
        }

        private async void ReceiptWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadReceiptAsync();
        }

        private async Task LoadReceiptAsync()
        {
            using var context =
                new AppDbContext();

            var order =
                await context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.RestaurantTable)
                    .FirstOrDefaultAsync(
                        o => o.Id == _orderId);

            if (order == null)
            {
                MessageBox.Show(
                    "Receipt could not be loaded.",
                    "RestaurantPOS",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();

                return;
            }

            OrderNumberTextBlock.Text =
                $"Order #{order.Id}";

            OrderDateTextBlock.Text =
                order.OrderDate.ToString(
                    "dd/MM/yyyy hh:mm tt");

            OrderTypeTextBlock.Text =
                $"Type: {order.OrderType}";

            if (order.OrderType == "Dine-In")
            {
                TableTextBlock.Text =
                    order.RestaurantTable?.TableNumber
                    ?? "-";
            }
            else
            {
                TableTextBlock.Text =
                    "Takeaway";
            }

            ReceiptItemsControl.ItemsSource =
                order.OrderItems;

            SubtotalTextBlock.Text =
                $"RM {order.Subtotal:N2}";

            TaxTextBlock.Text =
                $"RM {order.Tax:N2}";

            TotalTextBlock.Text =
                $"RM {order.Total:N2}";

            PaymentMethodTextBlock.Text =
                order.PaymentMethod;

            AmountReceivedTextBlock.Text =
                $"RM {order.AmountReceived:N2}";

            ChangeTextBlock.Text =
                $"RM {order.ChangeAmount:N2}";
        }

        private void PrintButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            PrintDialog printDialog =
                new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(
                    ReceiptBorder,
                    $"Receipt #{_orderId}");
            }
        }

        private void CloseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}