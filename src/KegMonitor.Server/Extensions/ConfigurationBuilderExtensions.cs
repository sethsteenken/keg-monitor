namespace KegMonitor.Server
{
    internal static class ConfigurationBuilderExtensions
    {
        public static IConfigurationRoot BuildConfiguration(
            this IConfigurationBuilder builder, 
            bool reloadOnChange = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder = builder.SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: reloadOnChange);

            // custom local json file allowed for local development settings to override
            // this local file should typically be omitted from source control
            builder = builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: reloadOnChange);
            
            return builder.Build();
        }
    }
}
