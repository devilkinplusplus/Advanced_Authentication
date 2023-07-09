using AdvancedAuth.DTOs;
using AdvancedAuth.ResponseParameters;

namespace AdvancedAuth.Services.Abstracts
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterUserDTO model);
        Task<LoginResponse> LoginUserAsync(string usernameOrEmail,string password);
        Task LogoutAsync();
        Task PasswordResetAsync(string email);
        Task<bool> VerifyResetTokenAsync(string resetToken, string userId);
        Task<UpdatePasswordResponse> UpdatePasswordAsync(string userId, string newPassword, string resetToken);
        bool CheckPassword(string password,string congirmPassword);
    }
}
