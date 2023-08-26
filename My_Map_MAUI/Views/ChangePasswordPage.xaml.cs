namespace My_Map_MAUI.Views;

public partial class ChangePasswordPage : ContentPage
{
	ChangePasswordViewModel viewModels;
	public ChangePasswordPage(ChangePasswordViewModel viewModel)
	{
		InitializeComponent();
		this.viewModels = viewModel;
		BindingContext = viewModel;
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