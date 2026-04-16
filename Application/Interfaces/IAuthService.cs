using Application.DTO;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(string username, string password);
        Task RegisterAsync(string username, string password);
        Task<AuthResponseDTO> RefreshAsync(string refreshToken);
    }
}
