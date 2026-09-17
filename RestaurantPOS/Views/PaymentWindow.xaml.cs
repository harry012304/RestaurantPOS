using System.Windows;

namespace RestaurantPOS.Views
{
    public partial class PaymentWindow : Window
    {
        private readonly decimal _total;

        public decimal AmountReceived { get; private set; }

        public PaymentWindow(decimal total)
        {
            InitializeComponent();

            _total = total;

            TotalTextBlock.Text = $"RM {_total:N2}";
        }

        private void AmountReceivedTextBox_TextChanged(
            object sender,
            System.Windows.Controls.TextChangedEventArgs e)
        {
            ErrorTextBlock.Text = string.Empty;

            if (!decimal.TryParse(
                AmountReceivedTextBox.Text,
                out decimal amount))
            {
                ChangeTextBlock.Text = "RM 0.00";
                return;
            }

            decimal change = amount - _total;

            if (change < 0)
            {
                ChangeTextBlock.Text = "RM 0.00";
                return;
            }

            ChangeTextBlock.Text =
                $"RM {change:N2}";
        }

        private void ConfirmPaymentButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ErrorTextBlock.Text = string.Empty;

            if (!decimal.TryParse(
                AmountReceivedTextBox.Text,
                out decimal amount))
            {
                ErrorTextBlock.Text =
                    "Please enter a valid amount.";

                return;
            }

            if (amount < _total)
            {
                ErrorTextBlock.Text =
                    "Cash received is less than the total.";

                return;
            }

            AmountReceived = amount;

            DialogResult = true;

            Close();
        }

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}