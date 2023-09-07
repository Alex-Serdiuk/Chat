using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class ChatUserVM : INotifyPropertyChanged
    {
        
        public ChatUser ModelChatUser { get; set; }
        public int Id {
            get { return ModelChatUser.Id; }
            set
            {
                ModelChatUser.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string Name {
            get { return ModelChatUser.Name; }
            set
            {
                ModelChatUser.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Login {
            get { return ModelChatUser.Login; }
            set
            {
                ModelChatUser.Login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        //public List<MessageData> Messages {
        //    get { return ModelChatUser.Messages; }
        //    set
        //    {
        //        ModelChatUser.Messages = value;
        //        OnPropertyChanged(nameof(Messages));
        //    }
        //}
        //public ChatUserVM()
        //{
        //    ModelChatUser.Messages = new List<MessageData>();
        //}
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not ChatUserVM) return false;
            if ((obj as ChatUserVM).ModelChatUser == null) return false;
            return ModelChatUser.Id.Equals((obj as ChatUserVM).ModelChatUser.Id);
        }
        public override int GetHashCode()
        {
            return ModelChatUser.Id.GetHashCode();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
