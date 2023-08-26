namespace My_Map_MAUI.Views;

public partial class SignUpPage : ContentPage
{
    SignUpViewModels viewModels;
	public SignUpPage(SignUpViewModels viewModels)
	{
		InitializeComponent();
        this.viewModels = viewModels;
        BindingContext = viewModels;
	}

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckName();
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