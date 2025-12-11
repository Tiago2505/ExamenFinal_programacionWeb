using examen_final.DTOs;
using examen_final.Models;

namespace examen_final.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto registerdto);
        Task<AuthResponseDto> Login(LoginDto logindto);
        Task<User?> GetUserById(string userId);
        Task<User?> GetUserByEmail(string email);
        string GenerateJwtToken(User user);
    }
}