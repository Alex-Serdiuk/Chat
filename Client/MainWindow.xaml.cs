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
        internal ChatClient _chatClient;

        public ObservableCollection<string> MessagessItems;
        // Ви можете використовувати ObservableCollection для автоматичного оновлення UI при змінах у списку користувачів
        public ObservableCollection<string> userList;
        List<ChatUser> users = new List<ChatUser>();
        
        
        //List<MessageData> messages = new List<MessageData>();



        public MainWindow(ChatUser me, ChatClient chatClient)
        {
            InitializeComponent();

            Me = me;
            _chatClient = chatClient;
            ClientLable.Content = "Client: " + me.Name.ToString();
            MessagessItems = new ObservableCollection<string>();
            userList = new ObservableCollection<string>();
            if (me is ChatUser)
            {
                Me = me;
                // Переключіться на UI чату
                //loginPanel.Visibility = Visibility.Collapsed;
                //userListBox.Visibility = Visibility.Visible;

                // Завантажте список користувачів
                LoadUserList(); //TODO refactor
            }
            else

            {
                MessageBox.Show("Authentication failed.");
            }
        }

        private ChatUser Me { get; set; }
        private ChatUser? receiver { get; set; }
        

   

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            userListBox_Selected();
            bool response= _chatClient.SendMessage(Me, receiver, messageTextBox.Text);
           
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

            LoadViewMessages();
        }


       
        private void LoadUserList()
        {
            userListBox.ItemsSource = null;
            userList.Clear();
            //userList.Add("Everyone");
            
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

        public int startId;

        public void LoadViewMessages()
        {
            userListBox_Selected();
            startId = 0; //TODO звідки брати Id першого повідомлення
            GetMessagesRequest request = new GetMessagesRequest();
            request = new GetMessagesRequest()
            {
                From = Me,
                To = receiver,
                AfterId = startId
            };
            messages = _chatClient.GetMessagesFromServer(request);

            messageListBox.ItemsSource = null;
            MessagessItems.Clear();

            foreach (MessageData message in messages)
            {
                string formattedMessage = $"{users.FirstOrDefault(user => user.Id == message.From.Id).Name} ({message.CreatedAt}): {message.Text}";
                MessagessItems.Add(formattedMessage);
            }
            messageListBox.ItemsSource = MessagessItems;
        }


        

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
            }
            //else if (selectedUserName == "Everyone")
            //{
            //    receiver = null;
            //}
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            LoadViewMessages();
        }
    }
}
