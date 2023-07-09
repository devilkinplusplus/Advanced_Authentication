using AdvancedAuth.DTOs;
using AdvancedAuth.Model;
using AdvancedAuth.ResponseParameters;
using AdvancedAuth.Services.Abstracts;
using AdvancedAuth.Validations;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AdvancedAuth.Services.Concretes
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMailService _mailService;
        private readonly IPasswordValidator<User> _passwordValidator;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IMailService mailService, IPasswordValidator<User> passwordValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _passwordValidator = passwordValidator;
        }
        public async Task<RegisterResponse> RegisterUserAsync(RegisterUserDTO model)
        {
            User user = new()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                BirthDate = model.BirthDate
            };

            GenerateUsername(user);

            ValidationResult results = await ValidateUserAsync(user);

            if (results.IsValid)
            {
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return new() { Succeeded = false, Errors = result.Errors.Select(x => x.Description).ToList() };

                return new() { Succeeded = true, Errors = null };
            }

            return new() { Succeeded = results.IsValid, Errors = results.Errors.Select(x => x.ErrorMessage).ToList() };
        }
        public async Task<LoginResponse> LoginUserAsync(string usernameOrEmail, string password)
        {
            User? user = await _userManager.FindByEmailAsync(usernameOrEmail);
            if (user is null)
                user = await _userManager.FindByNameAsync(usernameOrEmail);

            if (user is null)
                return new() { Succeeded = false, Errors = new List<string>() { "Incorrect email or username" } };

            SignInResult result = await _signInManager.PasswordSignInAsync(user, password, true, true);

            if (!result.Succeeded)
            {
                return new() { Succeeded = false, Errors = new List<string>() { "Username or password is incorrect,try again" } };
            }

            return new() { Succeeded = true };
        }
        public async Task LogoutAsync() => await _signInManager.SignOutAsync();
        private async Task<ValidationResult> ValidateUserAsync(User user)
        {
            UserValidation validations = new();
            return await validations.ValidateAsync(user);
        }
        private void GenerateUsername(User user)
        {
            Random rand = new();
            string userName = $"{user.LastName.ToLower()}.{rand.Next(0, 1000)}";

            bool condition = _userManager.Users.Any(x => x.UserName == userName);
            if (condition)
                GenerateUsername(user);

            user.UserName = userName;
        }

        public async Task PasswordResetAsync(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                byte[] tokenBytes = Encoding.UTF8.GetBytes(resetToken);
                resetToken = WebEncoders.Base64UrlEncode(tokenBytes);

                await _mailService.SendPasswordResetMailAsync(email, user.Id, resetToken);
            }
        }

        public async Task<bool> VerifyResetTokenAsync(string resetToken, string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                byte[] tokenBytes = WebEncoders.Base64UrlDecode(resetToken);
                resetToken = Encoding.UTF8.GetString(tokenBytes);

                return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider,
                                                                        "ResetPassword", resetToken);
            }

            return false;
        }

        public async Task<UpdatePasswordResponse> UpdatePasswordAsync(string userId, string newPassword, string resetToken)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                IdentityResult res = await _passwordValidator.ValidateAsync(_userManager, user, newPassword);

                if (res.Succeeded)
                {
                    byte[] tokenBytes = WebEncoders.Base64UrlDecode(resetToken);
                    resetToken = Encoding.UTF8.GetString(tokenBytes);
                    IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        return new() { Succeeded = true };
                    }
                    else
                        return new() { Succeeded = false };
                }
                return new() { Succeeded = false, Errors = res.Errors.Select(x => x.Description).ToList() };
            }
            return new() { Succeeded = false };
        }

        public bool CheckPassword(string password, string congirmPassword) => password == congirmPassword;
    }
}
