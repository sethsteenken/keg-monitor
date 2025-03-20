using MudBlazor;

namespace KegMonitor.Web
{
    public static class MudBlazorExtensions
    {
        public static SnackbarConfiguration SetConfigValues(this SnackbarConfiguration config)
        {
            config.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.PreventDuplicates = false;
            config.NewestOnTop = false;
            config.ShowCloseIcon = true;
            config.VisibleStateDuration = 10000;
            config.HideTransitionDuration = 500;
            config.ShowTransitionDuration = 500;
            config.SnackbarVariant = Variant.Filled;

            return config;
        }
    }
}
