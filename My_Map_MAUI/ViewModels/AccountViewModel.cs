using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.ViewModels
{
    public partial class AccountViewModel : DataOfMapViewModels
    {
        public AccountViewModel(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            IsBusy = false;
            IsFirstTimes = true;
            Title = "Account";
        }

        [RelayCommand]
        public async Task SignOutAsync()
        {
            if(IsBusy) return;
            IsBusy = true;
            await Shell.Current.GoToAsync("//SignInPage",true);
            IsBusy = false;
        }

        [RelayCommand]
        public async Task GoToChangePasswordPage()
        {
            if (IsBusy) return;
            IsBusy = true;
            await Shell.Current.GoToAsync("//AccountPage/ChangePasswordPage", true);
            IsBusy = false;
        }
    }
}
