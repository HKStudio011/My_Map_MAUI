namespace My_Map_MAUI.Views;

public partial class LocationPage : ContentPage
{
	LocationViewModels viewModels;

    public LocationPage(LocationViewModels viewModels)
	{
		InitializeComponent();
		this.viewModels = viewModels;
		BindingContext = viewModels;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!viewModels.IsFirstTimes)
        {
            await viewModels.LoadPinsAsync();
        }
        if (viewModels.IsFirstTimes)
        {
            await viewModels.LoadPinsAsync();
        }
        viewModels.IsFirstTimes = false;
    }
}