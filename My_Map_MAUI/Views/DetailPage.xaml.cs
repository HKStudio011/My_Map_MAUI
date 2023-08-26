namespace My_Map_MAUI.Views;

public partial class DetailPage : ContentPage
{
	DetailViewModels viewModels;

    public DetailPage(DetailViewModels viewModels)
	{
		InitializeComponent();
        this.viewModels = viewModels;
        BindingContext = viewModels;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModels.Init();
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckName();
    }

    private void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        viewModels.CheckShortDescription();
    }
}