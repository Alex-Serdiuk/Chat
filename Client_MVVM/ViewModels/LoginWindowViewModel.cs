using Client_MVVM.BaseViewModels;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Client_MVVM.ViewModels
{
    public class LoginWindowViewModel : NotifyPropertyChangedBase
    {
        private ChatClient _chatClient;
        private ChatUser? Me { get; set; }
        private LoginData? LoginMe { get; set; }
        public UserViewModel UserModel { get; set; }
        private readonly Window _loginWindow;

        public LoginWindowViewModel(Window loginWindow)
        {
            _loginWindow = loginWindow;
            _chatClient = new ChatClient("127.0.0.1", 12345);

            Me = new ChatUser { Id = 0, Name = "", Login = "" };
            LoginMe = new LoginData { Login = "", Password = "" };
            UserModel = new UserViewModel(Me, LoginMe);
            
        }

        public int Id
        {
            get => UserModel.Model.Id;
            set
            {
                UserModel.Model.Id = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Name
        {
            get => UserModel.Model.Name;
            set
            {
                UserModel.Model.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Login
        {
            get => UserModel.LoginModel.Login;
            set
            {
                UserModel.LoginModel.Login = value;
                UserModel.Model.Login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        public string Password
        {
            get => UserModel.LoginModel.Password;
            set
            {
                UserModel.LoginModel.Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand => new RelayCommand(x =>
        {

            var me = _chatClient.Login(LoginMe.Login, LoginMe.Password);
            if (me is ChatUser)
            {
                Me = me;

                // Створити та показати головне вікно чату
                MainWindow chatWindow = new MainWindow(Me, _chatClient);
                // Закрити вікно авторизації
                _loginWindow.Close();
                chatWindow.Show();
            }
            else
            {
                MessageBox.Show("Authentication failed.");
            }
        }, x => {
            if (string.IsNullOrEmpty(Login) ||
                string.IsNullOrEmpty(Password))
                return false;
            return true;
        });

        public ICommand RegisterCommand => new RelayCommand(x =>
        {
            var me = _chatClient.Register(Me.Name, LoginMe.Login, LoginMe.Password);

            if (me is ChatUser)
            {
                Me = me;

                // Створити та показати головне вікно чату
                MainWindow chatWindow = new MainWindow(Me, _chatClient);
                // Закрити вікно реєстрації
                _loginWindow.Close();
                chatWindow.Show();
            }
            else
            {
                MessageBox.Show("Registration failed.");
            }
        }, x => {
            if (string.IsNullOrEmpty(Name) ||
            string.IsNullOrEmpty(Login) ||
                string.IsNullOrEmpty(Password))
                return false;
            return true;
        });
    }
}
