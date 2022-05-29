namespace KegMonitor.Server
{
    internal class AuthSettings
    {
        public bool Enabled { get; set; }
        public IEnumerable<string>? AllowedClientIds { get; set; }
    }
}
