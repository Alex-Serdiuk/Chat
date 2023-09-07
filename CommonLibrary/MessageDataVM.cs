using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class MessageDataVM : INotifyPropertyChanged
    {
        public MessageData ModelMessageData { get; set; }
        public ChatUser From {
            get { return ModelMessageData.From; }
            set
            {
                ModelMessageData.From = value;
                OnPropertyChanged(nameof(From));
            }
        }
        public ChatUser To {
            get { return ModelMessageData.To; }
            set
            {
                ModelMessageData.To = value;
                OnPropertyChanged(nameof(To));
            }
        }
        public DateTime CreatedAt {
            get { return ModelMessageData.CreatedAt; }
            set
            {
                ModelMessageData.CreatedAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }
        public string? Text {
            get { return ModelMessageData.Text; }
            set
            {
                ModelMessageData.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not MessageDataVM) return false;
            if ((obj as MessageDataVM).ModelMessageData == null) return false;
            return ModelMessageData.CreatedAt.Equals((obj as MessageDataVM).ModelMessageData.CreatedAt);
        }
        public override int GetHashCode()
        {
            return ModelMessageData.CreatedAt.GetHashCode();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
