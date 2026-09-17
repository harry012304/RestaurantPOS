using System.Windows;
using System.Windows.Controls;
using RestaurantPOS.ViewModels;

namespace RestaurantPOS.Views
{
    public partial class LoginView : UserControl
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();

            _viewModel = new LoginViewModel();

            DataContext = _viewModel;
        }

        private void PasswordInput_PasswordChanged(
            object sender,
            RoutedEventArgs e)
        {
            _viewModel.Password =
                PasswordInput.Password;
        }

        private async void LoginButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            var user = await _viewModel.LoginAsync();

            if (user != null)
            {
                Application.Current.MainWindow.Content = new DashboardView(user);
            }
        }
    }
}