using Microsoft.Extensions.Logging;

namespace Claim_Macao_MobileGame
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // MainPage
            builder.Services.AddSingleton<View.MainPage>();
            builder.Services.AddSingleton<ViewModel.MainPageViewModel>();
            //GamePage
            builder.Services.AddSingleton<View.GamePage>();
            builder.Services.AddSingleton<ViewModel.GamePageViewModel>();

            return builder.Build();
        }
    }
}
