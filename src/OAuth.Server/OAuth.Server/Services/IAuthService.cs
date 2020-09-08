using OAuth.Server.Contracts;
using System.Threading.Tasks;

namespace OAuth.Server.Services
{
    public interface IAuthService
    {
        Task Register(RegisterAccountContract contract);

        Task CheckClientId(string clientId);

        Task CheckUserExists(LoginAccountContract contract);

        Task SetState(string state);

        Task<bool> CheckState(string state);

        Task<string> GenerateToken(TokenRequestContract contract);

        Task<string> GenerateAuthorizationCode(string userName, string clientId, string redirectUri);
    }
}
