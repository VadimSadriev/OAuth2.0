namespace OAuth.Server.Configuration
{
    public class AuthConfiguration
    {
        public AuthClient[] Clients { get; set; }
    }

    public class AuthClient
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
