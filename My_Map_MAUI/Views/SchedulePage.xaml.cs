namespace My_Map_MAUI.Views;

public partial class SchedulePage : ContentPage
{
	ScheduleViewModels viewModels;
    public SchedulePage(ScheduleViewModels viewModels)
	{
		InitializeComponent();
		this.viewModels = viewModels;
		BindingContext = viewModels;
	}
}