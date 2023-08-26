namespace My_Map_MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
#if IOS
        UIKit.UIDevice.CurrentDevice.SetValueForKey(Foundation.NSNumber.FromNInt((int)(UIKit.UIInterfaceOrientation.Portrait)), new Foundation.NSString("orientation"));
#endif
        }
    }
}