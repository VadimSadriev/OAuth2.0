using System;

namespace OAuth.Server.Data.Entities
{
    public class AuthCode
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public DateTimeOffset Expiration { get; set; }

        public string AccountId { get; set; }
        public Account Account { get; set; }
    }
}
