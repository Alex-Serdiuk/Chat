using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class LoginRegisterWindow : Window
    {
        private ChatClient _chatClient;
        private ChatUser? Me { get; set; }
        public LoginRegisterWindow()
        {
            InitializeComponent();
            
            _chatClient = new ChatClient("127.0.0.1", 12345);
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var me = _chatClient.Login(loginTextBox.Text, passwordBox.Password);

            if (me is ChatUser)
            {
                Me = me;

                // Створити та показати головне вікно чату
                MainWindow chatWindow = new MainWindow(Me, _chatClient);
                // Закрити вікно авторизації
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

                // Створити та показати головне вікно чату
                MainWindow chatWindow = new MainWindow(Me, _chatClient);
                // Закрити вікно реєстрації
                this.Close();
                chatWindow.Show();
            }
            else
            {
                MessageBox.Show("Registration failed.");
            }
        }
    }
}
