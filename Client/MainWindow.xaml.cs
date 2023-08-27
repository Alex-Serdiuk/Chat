using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatClient _chatClient;
        
        public MainWindow()
        {
            InitializeComponent();
			_chatClient = new ChatClient("127.0.0.1", 12345);

        }

        private ChatUser Me { get; set; }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var me = _chatClient.Login(usernameTextBox.Text, passwordBox.Password);

			if (me is ChatUser) {
                Me = me;
				// Переключіться на UI чату
				loginPanel.Visibility = Visibility.Collapsed;
				userListView.Visibility = Visibility.Visible;

				// Завантажте список користувачів
				//LoadUserList(); //TODO refactor
			} else

            {
				MessageBox.Show("Authentication failed.");
			}
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримайте повідомлення з текстового поля
            string message = messageTextBox.Text;

            // Відправте повідомлення на сервер
            byte[] data = Encoding.UTF8.GetBytes($"SEND|{message}");
            //_stream.Write(data, 0, data.Length);

            // Очистіть поле введення повідомлення
            messageTextBox.Text = string.Empty;
        }

        private void LoadUserList()
        {
            // Ви можете використовувати ObservableCollection для автоматичного оновлення UI при змінах у списку користувачів
            ObservableCollection<string> userList = new ObservableCollection<string>();

            // Отримайте список користувачів з сервера, наприклад, використовуючи TCP/IP
            // Ви маєте реалізувати логіку для отримання списку користувачів від сервера
            List<string> users = GetUsersFromServer();

            foreach (string user in users)
            {
                userList.Add(user);
            }

            // Прив'яжіть ObservableCollection до ListView на вашому інтерфейсі
            userListView.ItemsSource = userList;
        }

        private List<string> GetUsersFromServer()
        {
            List<string> users = new List<string>();

            try
            {
                // Встановіть з'єднання з сервером
                TcpClient client = new TcpClient("127.0.0.1", 12345);
                NetworkStream stream = client.GetStream();

                // Відправте запит на отримання списку користувачів
                byte[] data = Encoding.UTF8.GetBytes("GET_USERS");
                stream.Write(data, 0, data.Length);

                // Отримайте відповідь від сервера
                data = new byte[1024];
                int bytesRead = stream.Read(data, 0, data.Length);
                string response = Encoding.UTF8.GetString(data, 0, bytesRead);

                // Розібрати отриману відповідь і додати користувачів до списку
                if (!string.IsNullOrEmpty(response))
                {
                    string[] userArray = response.Split('|');
                    users.AddRange(userArray);
                }

                // Закрийте з'єднання
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting users from server: " + ex.Message);
            }

            return users;
        }

    }
}
