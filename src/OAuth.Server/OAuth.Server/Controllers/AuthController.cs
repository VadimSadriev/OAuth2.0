using Microsoft.AspNetCore.Mvc;
using OAuth.Server.Contracts;
using System.Threading.Tasks;

namespace OAuth.Server.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Register()
        {
            ViewData["Title"] = "Register an account";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterAccountContract contract)
        {
            return Ok();
        }
    }
}
