using My_Map_MAUI.ViewModels;

namespace My_Map_MAUI.Views;

public partial class ForgotPasswordPage : ContentPage
{
	ForgotPasswordViewModels viewModels;
	public ForgotPasswordPage(ForgotPasswordViewModels viewModels)
	{
		InitializeComponent();
		this.viewModels = viewModels;
		BindingContext = viewModels;
	}

    private void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckMail();
    }

    private void Entry_TextChanged_2(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckPassword();
    }

    private void Entry_TextChanged_3(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckPasswordConfirm();
    }
}