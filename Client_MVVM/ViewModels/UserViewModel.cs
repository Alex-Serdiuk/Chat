using Client_MVVM.BaseViewModels;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Client_MVVM.ViewModels
{
    public class UserViewModel : NotifyPropertyChangedBase
    {
        
        public UserViewModel(ChatUser chatUser, LoginData loginModel) 
        {
            Model = chatUser;
            LoginModel = loginModel;
        }

        public UserViewModel(ChatUser chatUser)
        {
            Model = chatUser;
            LoginModel = new LoginData { Login = chatUser.Login, Password = null};
        }

        public ChatUser Model { get; set; }
        public LoginData LoginModel { get; set; }

        public int Id
        {
            get => Model.Id;
            set
            {
                Model.Id = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Login
        {
            get => LoginModel.Login;
            set
            {
                LoginModel.Login= value;
                Model.Login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        public string Password
        {
            get => LoginModel.Password;
            set
            {
                LoginModel.Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

       
    }
}
