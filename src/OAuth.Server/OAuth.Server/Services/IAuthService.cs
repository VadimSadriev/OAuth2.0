using OAuth.Server.Contracts;
using System.Threading.Tasks;

namespace OAuth.Server.Services
{
    public interface IAuthService
    {
        Task Register(RegisterAccountContract contract);
    }
}
