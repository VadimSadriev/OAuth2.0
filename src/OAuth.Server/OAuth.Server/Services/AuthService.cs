using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Server.Configuration;
using OAuth.Server.Contracts;
using OAuth.Server.Data;
using OAuth.Server.Data.Entities;
using OAuth.Server.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IOptions<AuthConfiguration> _options;
        private readonly UserManager<Account> _userManager;
        private readonly IDistributedCache _cache;
        private readonly DataContext _context;

        public AuthService(DataContext context, UserManager<Account> userManager,
            IDistributedCache cache, IOptions<AuthConfiguration> options, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _cache = cache;
            _options = options;
            _context = context;
            _jwtOptions = jwtOptions;
        }

        public async Task Register(RegisterAccountContract contract)
        {
            // this query can have concurrency but since normalized username and email
            // columns have unique indexes then savechanges with same credentials
            // on different threads will cuz exception from db
            var existingUserName = await _userManager.FindByNameAsync(contract.UserName);

            if (existingUserName != null)
                throw new BaseException("Account with same user name already exists");

            var existingEmail = await _userManager.FindByEmailAsync(contract.Email);

            if (existingEmail != null)
                throw new BaseException("Account with same email already exists");

            var account = new Account
            {
                UserName = contract.UserName,
                Email = contract.Email,
            };

            var result = await _userManager.CreateAsync(account, contract.Password);

            if (!result.Succeeded)
            {
                var descriptions = result.Errors.Select(x => x.Description);

                var error = descriptions.Aggregate((curr, next) => $"{curr}{Environment.NewLine}{next}");

                throw new BaseException(error);
            }
        }

        public async Task SetState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new BaseException("state cannot be null or empty");

            var existing = await _cache.GetStringAsync($"state_{state}");

            if (existing != null)
                return;

            await _cache.SetStringAsync($"state_{state}", state,
                new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });
        }

        public async Task<bool> CheckState(string state)
        {
            var existing = await _cache.GetStringAsync($"state_{state}"); ;

            return existing == null;
        }

        public Task CheckClientId(string clientId)
        {
            if (!_options.Value.Clients.Any(x => x.ClientId == clientId))
                throw new BaseException("Wrong client id");

            return Task.CompletedTask;
        }

        public async Task<string> GenerateAuthorizationCode(string userName, string clientId, string redirectUri)
        {
            var code = Guid.NewGuid();

            var user = await _userManager.FindByNameAsync(userName);

            var authCode = new AuthCode
            {
                Id = code,
                ClientId = clientId,
                Expiration = DateTimeOffset.UtcNow.AddMinutes(10),
                RedirectUri = redirectUri,
                AccountId = user.Id
            };

            _context.Set<AuthCode>().Add(authCode);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new BaseException(ex.Message);
            }

            return code.ToString();
        }

        public async Task CheckUserExists(LoginAccountContract contract)
        {
            var user = await _userManager.FindByNameAsync(contract.UserName);

            if (user == null)
                throw new BaseException("Invalid user name or password");

            var validPass = await _userManager.CheckPasswordAsync(user, contract.Password);

            if (!validPass)
                throw new BaseException("Invalid user name or password");
        }

        public async Task<string> GenerateToken(TokenRequestContract contract)
        {
            await CheckClientId(contract.client_id);
            await CheckCode(contract.code);

            var token = GenerateTokenInternal(contract.grant_type, contract.code);

            return token;
        }

        private string GenerateTokenInternal(string grantType, string authCode)
        {
            switch (grantType)
            {
                case "access_token":
                    return GenerateJwt(authCode);
                default:
                    throw new ArgumentOutOfRangeException(nameof(grantType), "Invalid grant_type");
            }
        }

        public string GenerateJwt(string authCode)
        {
            var auth = _context.Set<AuthCode>().FirstOrDefault(x => x.Id == Guid.Parse(authCode))
                ?? throw new BaseException($"Authorization code: {authCode} not found");

            var now = DateTimeOffset.UtcNow;

            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(AuthConstants.Claims.JwtId, jti),
                new Claim(AuthConstants.Claims.JwtCreateDate, now.ToString()),
                new Claim(AuthConstants.Claims.UserId, auth.AccountId)
            };

            var signingCredentials = new SigningCredentials(
               new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey)),
               SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                signingCredentials: signingCredentials,
                expires: now.AddMinutes(_jwtOptions.Value.Expires).UtcDateTime
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        private async Task CheckCode(string code)
        {
            var authCode = await _context.Set<AuthCode>()
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(code));

            if (authCode == null)
                throw new BaseException("invalid authorization code");

            if (DateTimeOffset.UtcNow > authCode.Expiration)
                throw new BaseException("Authorization code has expired");
        }
    }
}
