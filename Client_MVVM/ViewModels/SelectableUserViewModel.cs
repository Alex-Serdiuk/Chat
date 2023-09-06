using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_MVVM.ViewModels
{
    public class SelectableUserViewModel : UserViewModel
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public SelectableUserViewModel(ChatUser user) : base(user)
        {
        }
    }

}
