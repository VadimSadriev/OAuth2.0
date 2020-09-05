using Microsoft.AspNetCore.Identity;
using System;

namespace OAuth.Server.Data.Entities
{
    public class Account : IdentityUser<Guid>
    {
        public DateTimeOffset CreateDate { get; set; }
    }
}
