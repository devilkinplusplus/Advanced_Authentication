using AdvancedAuth.DTOs;
using AdvancedAuth.ResponseParameters;
using AdvancedAuth.Services.Abstracts;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedAuth.Controllers
{

    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateReCaptcha]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            RegisterResponse response = await _authService.RegisterUserAsync(model);
            if (!response.Succeeded)
            {
                foreach (string error in response.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View();
            }
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(string usernameOrEmail, string password)
        {
            LoginResponse response = await _authService.LoginUserAsync(usernameOrEmail, password);

            if (!response.Succeeded)
            {
                foreach (string error in response.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View();
            }
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Json(true);
        }
    }
}
