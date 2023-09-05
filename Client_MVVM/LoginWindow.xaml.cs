using Client_MVVM.BaseViewModels;
using Client_MVVM.ViewModels;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_MVVM
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ChatClient _chatClient;
        private ChatUser? Me { get; set; }
        private LoginData? LoginMe { get; set; }
        //public UserViewModel UserModel { get; set; }
        public LoginWindow()
        {
            InitializeComponent();

            _chatClient = new ChatClient("127.0.0.1", 12345);

            Me = new ChatUser {Id=0, Name="", Login = "" };
            LoginMe = new LoginData {Login="",  Password=""};
            //UserModel = new UserViewModel(Me);
            //DataContext = UserModel;
            DataContext =  new UserViewModel(Me, LoginMe);
        }

        
    }
}
