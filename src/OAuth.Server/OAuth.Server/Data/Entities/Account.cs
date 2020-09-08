using Microsoft.AspNetCore.Identity;
using System;

namespace OAuth.Server.Data.Entities
{
    public class Account : IdentityUser<string>
    {
        public DateTimeOffset CreateDate { get; set; }
    }
}
