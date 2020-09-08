using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace OAuth.Server.Configuration
{
    public static class AuthConstants
    {
        public static class Claims
        {
            public const string UserId = ClaimTypes.NameIdentifier;

            public const string UserName = ClaimTypes.Name;

            public const string Email = ClaimTypes.Email;

            public const string JwtId = JwtRegisteredClaimNames.Jti;

            public const string JwtCreateDate = JwtRegisteredClaimNames.Iat;
        }
    }
}
