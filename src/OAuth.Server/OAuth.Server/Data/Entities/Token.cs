using OAuth.Server.Data.Enums;
using System;

namespace OAuth.Server.Data.Entities
{
    public class Token
    {
        public string Id { get; set; }
        public Typ Type { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public bool IsExpired { get; set; }
        public string AuthCodeId { get; set; }
        public AuthCode AuthCode { get; set; }
    }
}
