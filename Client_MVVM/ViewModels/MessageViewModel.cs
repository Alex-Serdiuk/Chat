using Client_MVVM.BaseViewModels;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_MVVM.ViewModels
{
    public class MessageViewModel : NotifyPropertyChangedBase
    {
        public MessageViewModel(MessageData model)
        {
            Model = model;
        }

        public MessageData Model { get; set; }

        public string Text
        {
            get => Model.Text;
            set
            {
                Model.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public UserViewModel From 
        { 
            get => new UserViewModel(Model.From); 
            set
            {
                Model.From = value.Model; 
                OnPropertyChanged(nameof(From));
            }
        }

        public DateTime CreatedAt => Model.CreatedAt;

        public ChatUser To
        {
            get => Model.To;
            set
            {
                Model.To = value;
                OnPropertyChanged(nameof(To));
            }
        }
    }
}
