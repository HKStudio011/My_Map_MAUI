using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Maui.ApplicationModel.Communication;

namespace My_Map_MAUI.ViewModels
{
    public partial class SignUpViewModels : BaseAccountProcessingViewModels
    {


        public SignUpViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            IsFirstTimes = true;
            IsBusy = false;
            Title = "Sign Up";
            IsNameReady = false;
            IsShowPass = false;
            IsEmailReady = false;
            IsPasswordReady = false;
            IsPasswordConfirmReady = false;
        }


        [RelayCommand]
        public async Task GoToSignInAsync()
        {
            if(IsBusy) { return; }
            IsBusy = true;

            await Shell.Current.GoToAsync("//SignInPage",true);

            IsBusy = false;
        }

        [RelayCommand]
        public async Task SignUpAsync()
        {
            if(IsBusy) { return; }
            try
            {
                IsBusy = true;

                var userList = await database.Database.Table<User>().Where(x => (x.Email == Email)).ToListAsync();
                if (userList.Count() == 0)
                {
                    IsWaitingOfVeryfy = false;
                    await SendVerificationCode(Email);
                    IsWaitingOfVeryfy = true;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Notification", "Email already exists", "Ok");
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

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.PropertyName == nameof(IsVeryfy) && IsVeryfy == true ) 
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    await AddUser();
                    IsVeryfy = false;
                });
            }
        }

        private async Task AddUser()
        {
            if( IsBusy ) { return; }
            try
            {
                IsBusy = true;
                var user = new User() { Name = Name, Email = Email, State = UserState.Activated};
                user.Password = SQLiteDatabase.EncodePassword(user.Email, Password);
                await database.SaveItemAsync(user);

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                string text = "Sign up success";
                ToastDuration duration = ToastDuration.Short;
                double fontSize = 15;

                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);

                await Shell.Current.GoToAsync("//SignInPage",true);
            }
            catch ( Exception ex ) 
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
