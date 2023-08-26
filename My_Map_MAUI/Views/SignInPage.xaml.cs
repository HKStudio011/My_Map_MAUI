using CommunityToolkit.Maui.Core;
using System.Security.Authentication.ExtendedProtection;

namespace My_Map_MAUI.Views;

public partial class SignInPage : ContentPage
{
	SignInViewModels viewModels;

    public SignInPage(SignInViewModels viewModels)
	{
		InitializeComponent();
		this.viewModels = viewModels;
		BindingContext = viewModels;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (viewModels.IsFirstTimes)
        {
            await viewModels.Init();
        }
        viewModels.IsFirstTimes = false;
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckMail();
    }

    private void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckPassword();
    }
}