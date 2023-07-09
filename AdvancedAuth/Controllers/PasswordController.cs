using AdvancedAuth.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedAuth.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IAuthService _authService;
        public PasswordController(IAuthService authService) => _authService = authService;

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                await _authService.PasswordResetAsync(email);
            }
            ViewData["alert"] = "We sent a mail for reset password, please check your email";
            return View();
        }


        [HttpGet("password/resetpassword/{userId}/{resetToken}")]
        public async Task<IActionResult> ResetPassword(string userId, string resetToken)
        {
            bool isTokenActive = await _authService.VerifyResetTokenAsync(resetToken, userId);
            if (isTokenActive)
            {
                ViewData["userId"] = userId;
                ViewData["resetToken"] = resetToken;
            }
            else
            {
                ViewData["msg"] = "This link is unavailable";
            }

            return View();
        }


        [HttpPost("password/resetpassword/{userId}/{resetToken}")]
        public async Task<IActionResult> ResetPassword(string userId, string resetToken, string newPassword, string confirmPassword)
        {
            bool res = _authService.CheckPassword(newPassword, confirmPassword);
            if (res)
            {
                var response = await _authService.UpdatePasswordAsync(userId, newPassword, resetToken);
                if (response.Succeeded)
                    return RedirectToAction("Login", "Auth");

                foreach (string error in response.Errors)
                {
                    ModelState.AddModelError("",error);
                }
                return View();
            }
            ModelState.AddModelError("", "Passwords don't match each other");
            return View();
        }

    }
}
