namespace KegMonitor.Web.Application
{
    public class HubUrlResolver
    {
        private readonly IConfiguration _configuration;

        public HubUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(string relativeUrl)
        {
            return $"{_configuration["Domain"].TrimEnd('/')}/{relativeUrl.TrimStart('/')}";
        }
    }
}
