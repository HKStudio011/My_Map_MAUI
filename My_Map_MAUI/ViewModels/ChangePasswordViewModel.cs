using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Map_MAUI.ViewModels
{
    public partial class ChangePasswordViewModel : ForgotPasswordViewModels
    {
        public ChangePasswordViewModel(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            Title = "Change Password";
            IsBusy = false;
            IsFirstTimes = true;
        }
    }
}
