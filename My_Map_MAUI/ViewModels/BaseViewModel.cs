
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;
using MimeKit.Text;

namespace My_Map_MAUI.ViewModels
{
    public partial class BaseViewModel : ObservableValidator
    {
        protected Serviecs.SQLiteDatabase database;

        [ObservableProperty]
        private bool isFirstTimes;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotConnectNetwork))]
        private bool isConnectNetwork;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string proccess;

        [ObservableProperty]
        private User currentUser;

        [ObservableProperty]
        private string verificationCode;

        [ObservableProperty]
        private string verificationMessage;

        [ObservableProperty]
        private bool isWaitingOfVeryfy;

        [ObservableProperty]
        private bool isVeryfy;

        [ObservableProperty]
        private string countDownTime;

        [ObservableProperty]
        private bool isShowUser;


        public bool IsNotConnectNetwork => !IsConnectNetwork;
        public bool IsNotBusy => !IsBusy;

        private IConnectivity connectivity;

        public BaseViewModel(IConnectivity connectivity, Serviecs.SQLiteDatabase database)
        {
            this.connectivity = connectivity;
            this.database = database;
            Proccess = "Loading";
            IsBusy = false;
            IsRefreshing = false;
            IsWaitingOfVeryfy = false;
            verificationCode = "";
            connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            isFirstTimes = true;
            IsVeryfy = false;
            IsShowUser = false;
            CheckNetword();

        }

        ~BaseViewModel()
        {
            connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            CheckNetword();
        }
        [RelayCommand]
        public void CheckNetword()
        {
            IsBusy = true;

            if (connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                IsConnectNetwork = true;
            }
            else
            {
                IsConnectNetwork = false;
            }
            IsBusy = false;
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                await Shell.Current.DisplayAlert("Notification", "Permission denied. Please, enable it in the settings", "Ok");
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                // Prompt the user with additional information as to why the permission is needed
                await Shell.Current.DisplayAlert("Notification", "Permissions needed to locate accurately", "Ok");
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }

        public async Task SendEmail(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HKStudio", "hkstudioentertainment@gmail.com"));
            message.To.Add(new MailboxAddress("User", email));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Plain)
            {
                Text = body
            };

            using (var client = new SmtpClient() { })
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto);

                await client.AuthenticateAsync("hkstudioentertainment@gmail.com", "pjclspqqkxvcxwqf");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

        }
    }
    public class StringCOnverterToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (((string)value).Length == 0) return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AllTrueMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || !targetType.IsAssignableFrom(typeof(bool)))
            {
                return false;
                // Alternatively, return BindableProperty.UnsetValue to use the binding FallbackValue
            }

            foreach (var value in values)
            {
                if (!(value is bool b))
                {
                    return false;
                    // Alternatively, return BindableProperty.UnsetValue to use the binding FallbackValue
                }
                else if (!b)
                {
                    return false;
                }
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (!(value is bool b) || targetTypes.Any(t => !t.IsAssignableFrom(typeof(bool))))
            {
                // Return null to indicate conversion back is not possible
                return null;
            }

            if (b)
            {
                return targetTypes.Select(t => (object)true).ToArray();
            }
            else
            {
                // Can't convert back from false because of ambiguity
                return null;
            }
        }
    }
}
