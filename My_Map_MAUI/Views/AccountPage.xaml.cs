namespace My_Map_MAUI.Views;

public partial class AccountPage : ContentPage
{
    AccountViewModel viewModels;
	public AccountPage(AccountViewModel viewModel)
	{
		InitializeComponent();
		this.viewModels = viewModel;
		BindingContext = viewModel;

    }

    private async void ViewCell_Tapped(object sender, EventArgs e)
    {
            await viewModels.GoToChangePasswordPage();

    }

    private async void ViewCell_Tapped_1(object sender, EventArgs e)
    {
            await viewModels.SignOutAsync();
    }

    private async void ViewCell_Tapped_2(object sender, EventArgs e)
    {
        await viewModels.ImportAsyc();
    }

    private void ViewCell_Tapped_3(object sender, EventArgs e)
    {

    }
}