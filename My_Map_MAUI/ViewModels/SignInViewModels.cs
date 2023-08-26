
using Microsoft.Win32;
using System.Xml.Linq;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Storage;

namespace My_Map_MAUI.ViewModels
{
    public partial class SignInViewModels : BaseAccountProcessingViewModels
    {

        [ObservableProperty]
        private bool isRememberMe;

        public SignInViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            IsBusy = false;
            IsFirstTimes = true;
            Title = "Sign In";
            IsShowPass = false;
            IsRememberMe = true;
            IsEmailReady = false;
            IsPasswordReady = false;
        }

        public async Task Init()
        {
            IsBusy = true;
            await database.Init();
            if(File.Exists(Path.Combine(FileSystem.Current.AppDataDirectory,"user.id")))
            {
                Email = File.ReadAllText(Path.Combine(FileSystem.Current.AppDataDirectory, "user.id"));
            }
            if (File.Exists(Path.Combine(FileSystem.Current.AppDataDirectory, "temp.token")))
            {
                string password = File.ReadAllText(Path.Combine(FileSystem.Current.AppDataDirectory, "temp.token"));
                string token = GetToken();
                if(SQLiteDatabase.EncodePassword(token,Email) == password)
                {
                    GlobalUsing.UserCurrent = (await database.Database.QueryAsync<User>($"select * from User where Email='{Email}'")).FirstOrDefault();
                    await GoToHomePage();
                } 
            }
            IsBusy = false;
        }


        [RelayCommand]
        public void RememberMe()
        {
            IsRememberMe = !IsRememberMe;
        }

        public string GetToken()
        {
            string token = "";

#if WINDOWS
                                RegistryKey localKey;
                                if (Environment.Is64BitOperatingSystem)
                                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                                else
                                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                                var RegKEY = localKey.OpenSubKey(@"SOFTWARE\Microsoft\SQMClient");
                                if (RegKEY != null)
                                {
                                    token = RegKEY.GetValue("MachineId").ToString().Trim();
                                }
#elif ANDROID        
                                token = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
#elif IOS
                                token = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.ToString();
#endif
            return token;
        }

        [RelayCommand]
        public async Task SignInAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;

                var kq = await database.Database.Table<User>().Where(x => (x.Email == Email)).ToListAsync();

                if (kq.Count() > 0)
                {
                    foreach (var val in kq)
                    {
                        if (val.Password == SQLiteDatabase.EncodePassword(Email, Password))
                        {
                            GlobalUsing.UserCurrent = (await database.Database.QueryAsync<User>($"select * from User where Email='{Email}' and Password = '{SQLiteDatabase.EncodePassword(Email, Password)}'")).FirstOrDefault();

                            File.WriteAllText(Path.Combine(FileSystem.Current.AppDataDirectory, "user.id"), val.Email);
                            if (IsRememberMe)
                            {
                                string token = GetToken();
                                File.WriteAllText(Path.Combine(FileSystem.Current.AppDataDirectory, "temp.token"), SQLiteDatabase.EncodePassword(token,val.Email));
                            }
                            EmailError = "";
                            await GoToHomePage();
                            break;
                        }
                        else
                        {
                            EmailError = "Email or password is incorrect";
                        }
                    }
                }
                else
                {
                    EmailError = "Email does not exist";
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task GoToHomePage()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            string text = "Sign in success";
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 15;
            var toast = Toast.Make(text, duration, fontSize);

            await toast.Show(cancellationTokenSource.Token);
            await Shell.Current.GoToAsync("//Home", true);
        }

        [RelayCommand]
        public async Task GoToForgotPasswordPage()
        {
            if (IsBusy) return;
            IsBusy = true;
            await Shell.Current.GoToAsync("//SignInPage/ForgotPasswordPage", true);
            IsBusy = false;
        }

        [RelayCommand]
        public async Task GoToSignUpPage()
        {
            if (IsBusy) return;
            IsBusy = true;
            await Shell.Current.GoToAsync("//SignUpPage", true);
            IsBusy = false;
        }
    }

}
