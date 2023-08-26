namespace My_Map_MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("LocationPage/DetailPage", typeof(DetailPage));
            Routing.RegisterRoute("MapPage/DetailPage", typeof(DetailPage));
            Routing.RegisterRoute("SignInPage/ForgotPasswordPage", typeof(ForgotPasswordPage));
            Routing.RegisterRoute("AccountPage/ChangePasswordPage", typeof(ChangePasswordPage));
        }

        protected override async void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            if (args.Current != null && args.Current.Location.OriginalString == "//Home/AccountPage" && args.Target.Location.OriginalString == "//SignInPage")
            {

                ShellNavigatingDeferral token = args.GetDeferral();

                var result = await Shell.Current.DisplayAlert("Notification", "Do you want to sign out?", "Yes", "No");
                if (!result)
                {
                    args.Cancel();
                }
                else
                {
                    if (File.Exists(Path.Combine(FileSystem.Current.AppDataDirectory, "temp.token")))
                    {
                        File.Delete(Path.Combine(FileSystem.Current.AppDataDirectory, "temp.token"));
                    }

                }
                token.Complete();
            }
        }
    }
}