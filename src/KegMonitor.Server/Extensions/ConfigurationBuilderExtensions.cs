namespace KegMonitor.Server
{
    internal static class ConfigurationBuilderExtensions
    {
        public static IConfigurationRoot BuildConfiguration(this IConfigurationBuilder builder,
                                                            string fileName = "appsettings.json",
                                                            string? directory = null,
                                                            bool reloadOnChange = false,
                                                            string? environmentName = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            builder = builder.SetBasePath(string.IsNullOrWhiteSpace(directory) ? Directory.GetCurrentDirectory() : directory)
                             .AddJsonFile(fileName, optional: false, reloadOnChange: reloadOnChange);

            if (fileName == "appsettings.json")
            {
                // include environment settings if present
                if (!string.IsNullOrWhiteSpace(environmentName) && environmentName.ToLower() != "local")
                    builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: reloadOnChange);

                // custom local json file allowed for local development settings to override
                // this local file should typically be omitted from source control
                builder = builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: reloadOnChange);
            }

            return builder.Build();
        }
    }
}
