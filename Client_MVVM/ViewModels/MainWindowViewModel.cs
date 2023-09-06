using Client_MVVM.BaseViewModels;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Threading;

namespace Client_MVVM.ViewModels
{
    class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ChatUser Me { get; set; }
        private ChatUser? receiver { get; set; }
        internal ChatClient _chatClient;

        //List<ChatUser> _users = new List<ChatUser>();
        //List<MessageData> _messages = new List<MessageData>();
        public GetDataResponse data;

        public MainWindowViewModel(ChatUser me, ChatClient chatClient)
        {
            Me = me;
            _chatClient = chatClient;
            data = new GetDataResponse();
            data.users = new List<ChatUser>();
            data.Messages = new List<MessageData>();
            NewMessage = new MessageViewModel(new MessageData { Text = "", From = Me, CreatedAt = DateTime.Now, To = new ChatUser { Id=0,  Name="", Login=""} });
            OnPropertyChanged(nameof(NewMessage));

            Task.Run(() => {
                while (true)
                {
                    UpdateUsersAndMessages(0);
                    Thread.Sleep(10000);
                }

            });
        }

        public ObservableCollection<MessageViewModel> Messages
        {
            get
            {
                var collection = new ObservableCollection<MessageViewModel>();
                data.Messages.ForEach(m => collection.Add(new MessageViewModel(m)));
                return collection;
            }
        }
        public ObservableCollection<UserViewModel> Users
        {
            get
            {
                var collection = new ObservableCollection<UserViewModel>();
                data.users.ForEach(u => collection.Add(new UserViewModel(u)));
                return collection;
            }
        }

        private UserViewModel _selectedUser;

        public UserViewModel SelectedUser
        {
            get =>_selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public int startId;
       
        private void UpdateUsersAndMessages(int startId)
        {
            
            data = _chatClient.GetUsersAndMessages(Me, startId);
            OnPropertyChanged(nameof(Messages));
            OnPropertyChanged(nameof(Users));
        }


        public MessageViewModel NewMessage { get; set; }
        public ICommand Send => new RelayCommand(x =>
        {
            bool response = _chatClient.SendMessage(NewMessage.Model.From, NewMessage.Model.To, NewMessage.Model.Text);

            if (response)
            {
                MessageBox.Show("Message sent!");
            }
            else
            {
                MessageBox.Show("Sending failed!");
            }
            NewMessage = new MessageViewModel(new MessageData { Text = "", From = Me, CreatedAt = DateTime.Now, To = new ChatUser {Id = 0, Name = "", Login = "" } });
            OnPropertyChanged(nameof(NewMessage));

        }, x => true);
    }
}
