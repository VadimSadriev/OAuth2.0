using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OAuth.Server.Contracts;
using OAuth.Server.Extensions;
using OAuth.Server.Services;
using System.Text;
using System.Threading.Tasks;

namespace OAuth.Server.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register an account";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterAccountContract contract)
        {
            await _authService.Register(contract);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromQuery]AuthorizationRequestContract contract)
        {
            ViewData["Title"] = "Login";
            await _authService.CheckClientId(contract.client_id);
            await _authService.SetState(contract.state);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromQuery]LoginRequestQueryContract loginRequest, [FromBody]LoginAccountContract credentials)
        {
            await _authService.CheckState(loginRequest.state);
            await _authService.CheckUserExists(credentials);

            var code = await _authService.GenerateAuthorizationCode(credentials.UserName, loginRequest.client_id, loginRequest.redirect_uri);

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", loginRequest.state);

            return Redirect($"{loginRequest.redirect_uri}{query.ToString()}");
        }

        [HttpGet]
        public async Task<IActionResult> Token([FromQuery]TokenRequestContract contract)
        {
            var token = await _authService.GenerateToken(contract);

            var responseObject = new
            {
                access_token = token,
                token_type = "Bearer"
            };

            var responseJson = responseObject.Serialize();

            await Response.WriteAsync(responseJson);

            return Redirect(contract.redirect_uri);
        }
    }
}
