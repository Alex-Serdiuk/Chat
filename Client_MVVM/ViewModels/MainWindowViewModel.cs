using Client_MVVM.BaseViewModels;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_MVVM.ViewModels
{
    class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private ChatUser Me { get; set; }
        private ChatUser? receiver { get; set; }
        internal ChatClient _chatClient;

        public MainWindowViewModel(ChatUser me, ChatClient chatClient)
        {
            Me = me;
            _chatClient = chatClient;
        }


    }
}
