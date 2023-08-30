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

        public ObservableCollection<MessageData> MessagessItems { set; get; }

        // Ви можете використовувати ObservableCollection для автоматичного оновлення UI при змінах у списку користувачів
        public ObservableCollection<string> userList = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
			_chatClient = new ChatClient("127.0.0.1", 12345);

            MessagessItems = new ObservableCollection<MessageData>();

        }

        private ChatUser Me { get; set; }
        private ChatUser? receiver { get; set; }
        

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var me = _chatClient.Login(usernameTextBox.Text, passwordBox.Password);

			if (me is ChatUser) {
                Me = me;
				// Переключіться на UI чату
				loginPanel.Visibility = Visibility.Collapsed;
				userListBox.Visibility = Visibility.Visible;

				// Завантажте список користувачів
				LoadUserList(); //TODO refactor
			} else

            {
				MessageBox.Show("Authentication failed.");
			}
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            userListBox_Selected();
            bool response= _chatClient.SendMessage(Me, receiver, messageTextBox.Text);
            // Отримайте повідомлення з текстового поля
            
            //// Відправте повідомлення на сервер
            //byte[] data = Encoding.UTF8.GetBytes($"SEND|{message}");
            ////_stream.Write(data, 0, data.Length);

            // Очистіть поле введення повідомлення
            messageTextBox.Text = string.Empty;

            if (response)
            {
                MessageBox.Show("Message sent!");
            }
            else
            {
                MessageBox.Show("Sending failed!");
            }
        }


       
        private void LoadUserList()
        {
            userList.Clear();
            userList.Add("Everyone");
            
            // Отримайте список користувачів з сервера, наприклад, використовуючи TCP/IP
            // Ви маєте реалізувати логіку для отримання списку користувачів від сервера
            users = _chatClient.GetUsersFromServer();

            foreach (ChatUser user in users)
            {
                userList.Add(user.Name);
            }

            // Прив'яжіть ObservableCollection до ListView на вашому інтерфейсі
            userListBox.ItemsSource = userList;
        }


        List<ChatUser>? users = new List<ChatUser>();

        private void userListBox_Selected()
        {
            // Отримайте вибраний елемент зі списку користувачів
            string? selectedUserName = userListBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedUserName) && selectedUserName != "Everyone")
            {
                // Знайдіть відповідного ChatUser за ім'ям
                receiver = users.FirstOrDefault(user => user.Name == selectedUserName);

                if (receiver != null)
                {
                    // Виведіть дані про обраного користувача
                    MessageBox.Show($"User ID: {receiver.Id}\nName: {receiver.Name}\nLogin: {receiver.Login}");
                }
            }else if (selectedUserName == "Everyone")
            {
                receiver = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
