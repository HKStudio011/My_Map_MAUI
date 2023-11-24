using CommunityToolkit.Maui.Maps;
using Microsoft.Extensions.Logging;

namespace My_Map_MAUI;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseMauiCommunityToolkitMaps("API KEY BING MAP")
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<Serviecs.SQLiteDatabase>();

        builder.Services.AddSingleton<MapPage>();
        builder.Services.AddSingleton<LocationPage>();
        builder.Services.AddTransient<DetailPage>();
        builder.Services.AddTransient<SignInPage>();
        builder.Services.AddTransient<SignUpPage>();
        builder.Services.AddTransient<ForgotPasswordPage>();
        builder.Services.AddSingleton<AccountPage>();
        builder.Services.AddTransient<ChangePasswordPage>();
        builder.Services.AddTransient<SchedulePage>();



        builder.Services.AddSingleton<MapViewModels>();
        builder.Services.AddSingleton<LocationViewModels>();
        builder.Services.AddTransient<DetailViewModels>();
        builder.Services.AddTransient<SignInViewModels>();
        builder.Services.AddTransient<SignUpViewModels>();
        builder.Services.AddTransient<ForgotPasswordViewModels>();
        builder.Services.AddSingleton<AccountViewModel>();
        builder.Services.AddTransient<ChangePasswordViewModel>();
        builder.Services.AddTransient<ScheduleViewModels>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

}