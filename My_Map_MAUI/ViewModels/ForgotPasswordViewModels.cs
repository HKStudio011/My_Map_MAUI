using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.Communication;

namespace My_Map_MAUI.ViewModels
{
    public partial class ForgotPasswordViewModels : BaseAccountProcessingViewModels
    {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotConfirm))]
        private bool isConfirm;

        public bool IsNotConfirm => !IsConfirm;

        public ForgotPasswordViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            Title = "Forgot Password";
            IsBusy = false;
            IsFirstTimes = true;
            IsShowPass = false;
            IsEmailReady = false;
            IsPasswordReady = false;
            IsPasswordConfirmReady = false;
            isConfirm = false;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(IsVeryfy) && IsVeryfy == true)
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    IsWaitingOfVeryfy = false;
                    IsConfirm = true;
                    IsVeryfy = false;
                });
            }
        }

        [RelayCommand]
        public async Task Confirm()
        {
            if (IsBusy) { return; }
            try
            {
                IsBusy = true;

                var userList = await database.Database.Table<User>().Where(x => (x.Email == Email)).ToListAsync();
                if (userList.Count() > 0)
                {
                    IsWaitingOfVeryfy = false;
                    await SendVerificationCode(Email);
                    IsWaitingOfVeryfy = true;
                }
                else if(userList.Count() <= 0)
                {
                    await Shell.Current.DisplayAlert("Notification", "Email not exists", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("!!!ERORR!!!", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task UpdatePass()
        {
            if (IsBusy) { return; }
            try
            {
                IsBusy = true;
                var userList = GlobalUsing.UserCollection.Where(p => p.Email == Email).ToList();
                if (userList[0].Password != SQLiteDatabase.EncodePassword(userList[0].Email,Password))
                {
                    userList[0].Password = SQLiteDatabase.EncodePassword(userList[0].Email, Password);
                    await database.SaveItemAsync(userList[0]);
                    GlobalUsing.UserCollection = new ObservableCollection<User>(await database.GetAllUserAsync());
                    await Shell.Current.GoToAsync("//SignInPage",true);
                }
                else 
                {
                    await Shell.Current.DisplayAlert("Notification", "Password is old password", "Ok");
                }

            }
            catch (Exception ex) 
            {
                await Shell.Current.DisplayAlert("!!!ERORR!!!", ex.Message, "Ok");
            }
            finally
            {
                IsWaitingOfVeryfy = false;
                IsBusy = false;
            }
        }

    }
}
