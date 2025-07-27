using DiligenciaProveedores.Application.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
namespace DiligenciaProveedores.Application.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterRequestDto request);
        Task<IdentityResult> RegisterAdminAsync(RegisterRequestDto request);
        Task<AuthResponseDto?> LoginUserAsync(LoginRequestDto request);
        Task<bool> VerifyTokenAsync(string token);
    }
}