﻿using Client.Converters;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginRegisterWindow.xaml
    /// </summary>
    public partial class LoginRegisterWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public ObservableCollection<ChatUserVM> Users { get; set; }
        private ChatClient _chatClient;
        private ChatUser? Me { get; set; }
        public LoginRegisterWindow()
        {
            InitializeComponent();
            DataContext = this;
            _chatClient = new ChatClient("127.0.0.1", 12345);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            string text = loginTextBox.Text;
            string password = passwordBox.Password;
            bool isButtonEnabled = !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(password);
            LoginButton.IsEnabled = isButtonEnabled;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var me = _chatClient.Login(loginTextBox.Text, passwordBox.Password);
            if (me is ChatUser)
            {
                Me = me;
                Users = _chatClient.GetUsersFromServer();
                MainWindow chatWindow = new MainWindow(Me, _chatClient);
                chatWindow.Users = Users;
                this.Close();
                chatWindow.Show();
            }
            else
            {
                MessageBox.Show("Authentication failed.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var me = _chatClient.Register(usernameTextBoxReg.Text, loginTextBoxReg.Text, passwordBoxReg.Password);
            if (me is ChatUser)
            {
                Me = me;
                Users = _chatClient.GetUsersFromServer();
                MainWindow chatWindow = new MainWindow(Me, _chatClient);
                chatWindow.Users = Users;
                this.Close();
                chatWindow.Show();
            }
            else
            {
                MessageBox.Show("Registration failed.");
            }
        }

        private void loginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonState();
        }

        private void UpdateButtonStateReg()
        {
            string textLogin = loginTextBoxReg.Text;
            string textName = usernameTextBoxReg.Text;
            string password = passwordBoxReg.Password;

            bool isButtonEnabled = !string.IsNullOrEmpty(textName) && !string.IsNullOrEmpty(textLogin) && !string.IsNullOrEmpty(password);

            RegisterButton.IsEnabled = isButtonEnabled;
        }

        private void usernameTextBoxReg_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonStateReg();
        }

        private void passwordBoxReg_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateButtonStateReg();
        }

        private void loginTextBoxReg_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonStateReg();
        }
    }
}
