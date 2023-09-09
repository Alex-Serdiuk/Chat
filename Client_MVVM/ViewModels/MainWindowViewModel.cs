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

        List<ChatUser> _users = new List<ChatUser>();
        List<MessageData> _messages = new List<MessageData>();
        public GetDataResponse data;

        public MainWindowViewModel(ChatUser me, ChatClient chatClient)
        {
            Me = me;
            _chatClient = chatClient;
            data = new GetDataResponse();
            data.users = new List<ChatUser>();
            data.Messages = new List<MessageData>();
            _newMessage = new MessageViewModel(new MessageData { Text = "", From = Me, CreatedAt = DateTime.Now, To = new ChatUser { Id = 0, Name = "", Login = "" } });
            NewMessage = _newMessage;
            OnPropertyChanged(nameof(NewMessage));
            OnPropertyChanged(nameof(SelectedUser));

            Task.Run(() => {
                while (true)
                {
                    //_tempUser = SelectedUser;
                    //_users.Clear();
                    UpdateUsers(0);
                    //UpdateUsersAndMessages(0);
                    Thread.Sleep(5000);
                }

            });
        }

        public ObservableCollection<MessageViewModel> Messages
        {
            get
            {
                var collection = new ObservableCollection<MessageViewModel>();
                if(SelectedUser!=null)
                    _messages=SelectedUser.Model.Messages;
                _messages.ForEach(m => collection.Add(new MessageViewModel(m)));
                return collection;
            }
        }
        public ObservableCollection<UserViewModel> Users
        {
            get
            {
                var collection = new ObservableCollection<UserViewModel>();
                //_users.ForEach(u => collection.Add(new UserViewModel(u)));
                foreach (var user in _users)
                {
                    var userViewModel = new UserViewModel(user);
                    collection.Add(userViewModel);

                    // Если выбранный пользователь все еще существует в списке,
                    // установите его как SelectedUser.
                    if (SelectedUser != null && SelectedUser.Model.Id == user.Id)
                    {
                        SelectedUser = userViewModel;
                    }
                }
                return collection;
            }
        }

        //private UserViewModel _tempUser;
        private UserViewModel _selectedUser;
        //private UserViewModel _newUser;

        public UserViewModel SelectedUser
        {
            get =>_selectedUser;
            set
            {
                    _selectedUser = value;
                    //_newUser = Users.FirstOrDefault(u => u.Id == _tempUser.Id);
                    //if(_newUser!=null)
                    //{
                    //    _selectedUser = _newUser;
                    //}
                

                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(Messages));
                OnPropertyChanged(nameof(NewMessage));
            }
        }

        public int startId;
       
        private void UpdateUsersAndMessages(int startId)
        {
            
            data = _chatClient.GetUsersAndMessages(Me, startId);
            OnPropertyChanged(nameof(Messages));
            OnPropertyChanged(nameof(Users));
        }

        private void UpdateUsers(int startId)
        {

            _users = _chatClient.GetUsersFromServer(Me, startId);
            
            OnPropertyChanged(nameof(Users));
        }

        public MessageViewModel _newMessage;
        public MessageViewModel NewMessage 
        {
            get => _newMessage;
            set
            {
                _newMessage = new MessageViewModel(new MessageData { Text = NewMessage.Text, From = Me, CreatedAt = DateTime.Now, To = new ChatUser { Id = (SelectedUser != null) ? SelectedUser.Id : 0, Name = (SelectedUser != null) ? SelectedUser.Name : "", Login = (SelectedUser != null) ? SelectedUser.Login : "" } }); ;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(NewMessage));
            }
        }

        public ICommand Send => new RelayCommand(x =>
        {
            bool response=false;
            if (SelectedUser!=null)
            {
                response = _chatClient.SendMessage(NewMessage.Model.From, SelectedUser.Model, NewMessage.Text);
            
            }
            if (response)
            {
                //MessageBox.Show("Message sent!");
                UpdateUsers(0);
            }
            else
            {
                MessageBox.Show("Sending failed!");
            }
            NewMessage.Text = "";
            
            OnPropertyChanged(nameof(NewMessage));

        }, x => {
            if (string.IsNullOrEmpty(NewMessage.Text) ||
                (SelectedUser==null))
                return false;
            return true;
        });
    }
}
