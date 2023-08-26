using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.Communication;

namespace My_Map_MAUI.ViewModels
{
    public partial class BaseAccountProcessingViewModels : BaseViewModel
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotShowPass))]
        private bool isShowPass;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotShowPassConfirm))]
        private bool isShowPassConfirm;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "{0} can not be blank")]
        private string name;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "{0} can not be blank")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-_]+(\.[a-zA-Z]{2,})+$", ErrorMessage = "Must be email")]
        private string email;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "{0} can not be blank")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "{0} is 8 to 32 characters long")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$", ErrorMessage = "Password includes: numbers, lowercase characters, uppercase characters, special characters")]
        private string password;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Password confirm can not be blank")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Password confirm is 8 to 32 characters long")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$", ErrorMessage = "Password confirm includes: numbers, lowercase characters, uppercase characters, special characters")]
        private string passwordConfirm;

        [ObservableProperty]
        private string nameError;

        [ObservableProperty]
        private string emailError;

        [ObservableProperty]
        private string passwordError;

        [ObservableProperty]
        private string passwordConfirmError;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotNameReady))]
        private bool isNameReady;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotEmailReady))]
        private bool isEmailReady;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotPasswordReady))]
        private bool isPasswordReady;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotPasswordConfirmReady))]
        private bool isPasswordConfirmReady;

        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _startTime;
        private const int _duration = 60000;
        private string verificationCodeSended;
        private bool IsCountingDown;

        public bool IsNotNameReady => !IsNameReady;
        public bool IsNotPasswordReady => !IsPasswordReady;
        public bool IsNotPasswordConfirmReady => !IsPasswordConfirmReady;
        public bool IsNotEmailReady => !IsEmailReady;
        public bool IsNotShowPass => !IsShowPass;
        public bool IsNotShowPassConfirm => !IsShowPassConfirm;
        public BaseAccountProcessingViewModels(IConnectivity connectivity, SQLiteDatabase database) : base(connectivity, database)
        {
            IsCountingDown = false;
            IsFirstTimes = true;
            IsBusy = false;
            Title = "";
            IsNameReady = false;
            IsShowPass = false;
            IsEmailReady = false;
            IsPasswordReady = false;
            IsPasswordConfirmReady = false;
        }

        public async Task SendVerificationCode(string email)
        {
            var temp = email.Split('@');
            string hideEmail;
            if (temp[0].Length > 2)
            {
                hideEmail = temp[0].Substring(0, 2) + "**" + "@" + temp[1];
            }
            else
            {
                hideEmail = temp[0].Substring(0, 1) + "**" + "@" + temp[1];
            }
            VerificationCode = "";
            verificationCodeSended = "";

            for (int i = 0; i < 8; i++)
            {
                var rand = new Random();

                verificationCodeSended += rand.Next(0, 10);
            }

            string subject = "My Maps - Verify your email";
            string body = $"""
                    Please use the following verification code for My Map account {hideEmail}.

                    Verification code: {verificationCodeSended}

                    If you don't recognize My Map account {hideEmail}, you can ignore this email.
                    Thanks,
                    My Map team
                    """;

            await SendEmail(email, subject, body);

        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(IsWaitingOfVeryfy) && IsWaitingOfVeryfy == true)
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    await VerifyingCode();
                });

            }
        }

        private async Task VerifyingCode()
        {
            if (IsBusy) return;
            try
            {
                if (!IsConnectNetwork)
                {
                    await Shell.Current.DisplayAlert("!!!ERROR!!!", "No network. Please try again", "Ok");
                    return;
                }
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }

                _cancellationTokenSource = new CancellationTokenSource();


                IsVeryfy = await CountDown();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("!!!ERROR!!!", ex.Message, "Ok");
            }
            finally
            {
                ;
            }
        }

        private async Task<bool> CountDown()
        {
            _startTime = DateTime.Now;
            int secondsRemaining = 60;
            IsCountingDown = true;

            Func<bool> func = () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested || secondsRemaining > 0)
                {
                    var elapsedTime = (DateTime.Now - _startTime);
                    secondsRemaining = (int)(_duration - elapsedTime.TotalMilliseconds) / 1000;
                    CountDownTime = $"{secondsRemaining}";
                    if (CheckVerificationCode())
                    {
                        IsCountingDown = false;
                        return true;
                    }
                    if (secondsRemaining == 0)
                    {
                        IsCountingDown = false;
                        return false;
                    }
                }
                IsCountingDown = false;
                return false;
            };

            Task<bool> task = new Task<bool>(func);
            task.Start();
            await task;
            return task.Result;
        }

        private bool CheckVerificationCode()
        {
            if (VerificationCode != null && VerificationCode.Length > 0)
            {
                if (VerificationCode == verificationCodeSended && IsCountingDown)
                {
                    VerificationMessage = "";
                    return true;
                }
                VerificationMessage = "The verification code is not correct";
            }
            return false;
        }

        [RelayCommand]
        public async Task ReSendCodeAsync(string email)
        {
            if (IsBusy) { return; }
            try
            {
                IsWaitingOfVeryfy = false;
                IsBusy = true;
                await SendVerificationCode(email);
                IsWaitingOfVeryfy = true;
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
        public async Task CancelVerify()
        {
            if (IsBusy) { return; }
            try
            {
                IsBusy = true;
                IsWaitingOfVeryfy = false;
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

        public void CheckName()
        {
            ValidateProperty(Name, nameof(Name));
            if (HasErrors)
            {
                IsNameReady = false;
                NameError = string.Join(Environment.NewLine, GetErrors().Where(p => p.MemberNames.All(q => q == nameof(Name))).Select(e => e.ErrorMessage));
                if (NameError == "")
                {
                    IsNameReady = true;
                }
            }
            else
            {
                NameError = "";
                IsNameReady = true;
            }
        }

        public void CheckMail()
        {
            ValidateProperty(Email, nameof(Email));
            if (HasErrors)
            {
                IsEmailReady = false;
                EmailError = string.Join(Environment.NewLine, GetErrors().Where(p => p.MemberNames.All(q => q == nameof(Email))).Select(e => e.ErrorMessage));
                if (EmailError == "")
                {
                    IsEmailReady = true;
                }
            }
            else
            {
                EmailError = "";
                IsEmailReady = true;
            }
        }

        public void CheckPassword()
        {

            ValidateProperty(Password, nameof(Password));
            if (HasErrors)
            {
                IsPasswordReady = false;
                PasswordError = string.Join(Environment.NewLine, GetErrors().Where(p => p.MemberNames.All(q => q == nameof(Password))).Select(e => e.ErrorMessage));
                if (PasswordError == "")
                {
                    IsPasswordReady = true;
                    if (Password != PasswordConfirm && (PasswordConfirm != null && PasswordConfirm != ""))
                    {
                        IsPasswordConfirmReady = false;
                        PasswordConfirmError = "Password must be the same";
                    }
                    else
                    {
                        IsPasswordConfirmReady = true;
                        PasswordConfirmError = "";
                    }
                }
            }
            else
            {
                PasswordError = "";
                IsPasswordReady = true;
                if (Password != PasswordConfirm && (PasswordConfirm != null && PasswordConfirm != ""))
                {
                    IsPasswordConfirmReady = false;
                    PasswordConfirmError = "Password must be the same";
                }
                else
                {
                    IsPasswordConfirmReady = true;
                    PasswordConfirmError = "";
                }
            }


        }
        public void CheckPasswordConfirm()
        {
            if (Password != PasswordConfirm)
            {
                IsPasswordConfirmReady = false;
                PasswordConfirmError = "Password must be the same";
            }
            else
            {
                IsPasswordConfirmReady = true;
                PasswordConfirmError = "";
            }

            //ValidateProperty(PasswordConfirm, nameof(PasswordConfirm));
            //if (HasErrors)
            //{
            //    IsPasswordConfirmReady = false;
            //    PasswordConfirmError = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));
            //}
            //else
            //{
            //    PasswordConfirmError = "";
            //    IsPasswordConfirmReady = true;
            //}
        }


        [RelayCommand]
        public void HidePassword()
        {
            IsShowPass = !IsShowPass;
        }

        [RelayCommand]
        public void HidePasswordConfirm()
        {
            IsShowPassConfirm = !IsShowPassConfirm;
        }
    }
}
