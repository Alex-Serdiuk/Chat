using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }              
        public ChatClient _chatClient;
        public int startId;
        public Timer timer;

        public ChatUserVM Me { get; set; }
        public ChatUserVM? _receiver;
        public ChatUserVM receiver
        {
            get => _receiver;
            set
            {
                _receiver = value;
                timer = new Timer(UpdateMessages, null, 0, 500);
                OnPropertyChanged(nameof(receiver));
            }
        }
        public ObservableCollection<MessageDataVM> Messages { get; set; }
        public ObservableCollection<ChatUserVM> Users { get; set; }        
        public MainWindow(ChatUser me, ChatClient chatClient)
        {
            InitializeComponent();
            DataContext = this;            
            _chatClient = chatClient;
            ClientLable.Content = me.Name.ToString();
            if (me is ChatUser)
            {
                Me = new ChatUserVM { ModelChatUser = me };
            }
            else
            {
                MessageBox.Show("Authentication failed.");
            }
        }    
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            bool response = _chatClient.SendMessage(Me.ModelChatUser, receiver.ModelChatUser, messageTextBox.Text);   
            messageTextBox.Text = string.Empty;
        }
        private void UpdateMessages(object obj)
        {
            startId = 0;
            GetMessagesRequest request = new GetMessagesRequest();
            request = new GetMessagesRequest()
            {
                From = Me.ModelChatUser,
                To = receiver.ModelChatUser,
                AfterId = startId
            };
            Messages = _chatClient.GetMessagesFromServer(request);
            for (int i = 0; i < Messages.Count(); i++)
            {
                if (Messages[i].From.Name == Me.Name)
                {
                    Messages[i].curUser = true;                    
                }
                else
                    Messages[i].curUser = false;
            }
            OnPropertyChanged(nameof(Messages));
        }
    }
}
