using RestaurantPOS.Models;
using RestaurantPOS.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantPOS.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _message = string.Empty;

        public LoginViewModel()
        {
            _authService = new AuthService();
        }

        public string Username
        {
            get => _username;

            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;

            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;

            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public async Task<User?> LoginAsync()
        {
            Message = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                Message = "Please enter username and password.";
                return null;
            }

            var user = await _authService.LoginAsync(
                Username,
                Password);

            if (user == null)
            {
                Message = "Invalid username or password.";
                return null;
            }

            return user;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}