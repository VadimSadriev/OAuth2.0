namespace OAuth.Server.Contracts
{
    public class LoginRequestQueryContract
    {
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
        public string state { get; set; }
    }
}
