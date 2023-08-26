
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;
using Location = Microsoft.Maui.Devices.Sensors.Location;
using Microsoft.Maui.Devices.Sensors;

namespace My_Map_MAUI.Views;

public partial class MapPage : ContentPage
{
	MapViewModels viewModels;   

    public MapPage(MapViewModels viewModels)
	{
		InitializeComponent();
		this.viewModels = viewModels;
		BindingContext = viewModels;

	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if(!viewModels.IsFirstTimes)
        {
            await viewModels.LoadPinsAsync();
        }
        if(viewModels.IsFirstTimes)
        {
            await viewModels.LoadPinsAsync();
            await viewModels.CheckAndRequestLocationPermission();
            await viewModels.GetCurrentLocation();
            if (viewModels.CurrentLocation != null)
            {
                map.MoveToRegion(new MapSpan(viewModels.CurrentLocation, 0.01, 0.01));
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                string text = "The current location has been determined";
                ToastDuration duration = ToastDuration.Short;
                double fontSize =15 ;
                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);
            }          
            else
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                string text = "Unable to determine current location";
                ToastDuration duration = ToastDuration.Short;
                double fontSize = 15;

                var toast = Toast.Make(text, duration, fontSize);

                await toast.Show(cancellationTokenSource.Token);
            }
        }
        viewModels.IsFirstTimes = false;
    }

    private async void map_MapClicked(object sender, MapClickedEventArgs e)
    {
        var result = await Shell.Current.DisplayAlert("Notification", "Do you want to pin?", "Yes", "No");
        if(result) 
        {
            await viewModels.GoToDetailCreatePinAsync(e.Location);
        }
    }

    private void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        
    }

    private void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {

    }
}