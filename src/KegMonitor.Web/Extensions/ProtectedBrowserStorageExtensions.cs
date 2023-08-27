using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace KegMonitor.Web
{
    public static class ProtectedBrowserStorageExtensions
    {
        public const bool DefaultDarkMode = true;

        public static async Task<bool> GetOrSetDarkThemeAsync(
            this ProtectedBrowserStorage storage, bool value)
        {
            var result = await storage.GetAsync<bool>("DarkMode");
            if (result.Success)
                return result.Value;
            
            await storage.SetAsync("DarkMode", value);
            return value;
        }

        public static async Task SetDarkThemeAsync(
            this ProtectedBrowserStorage storage, bool value)
        {
            await storage.SetAsync("DarkMode", value);
        }
    }
}
