using Microsoft.AspNetCore.Identity;
using OAuth.Server.Contracts;
using OAuth.Server.Data.Entities;
using OAuth.Server.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Account> _userManager;

        public AuthService(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        public async Task Register(RegisterAccountContract contract)
        {
            // this query can have concurrency but since normalized username and email
            // columns have unique indexes then savechanges with same credentials
            // on different threads will cuz exception from db
            var existingUserName = await _userManager.FindByNameAsync(contract.UserName);

            if (existingUserName != null)
                throw new BaseException("Account with same username already exists");

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
    }
}
