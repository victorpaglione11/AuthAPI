using Application.DTO;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _hasher;

        public AuthService(
            IUserRepository repo,
            ITokenService tokenService,
            IPasswordHasher hasher)
        {
            _repo = repo;
            _tokenService = tokenService;
            _hasher = hasher;
        }
        public async Task RegisterAsync(string username, string password)
        {
            var hash = _hasher.Hash(password);

            var user = new User
            {
                Username = username,
                PasswordHash = hash,
                Role = "User"
            };

            await _repo.AddAsync(user);
        }

        public async Task<AuthResponseDTO> LoginAsync(string username, string password)
        {
            var user = await _repo.GetByUsernameAsync(username);

            if (user == null || !_hasher.Verify(password, user.PasswordHash))
                throw new InvalidCredentialsException();

            var accessToken = _tokenService.GenerateToken(user.Username, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshEntity = new UserRefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _repo.SaveRefreshTokenAsync(refreshEntity);

            return new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }


        public async Task<AuthResponseDTO> RefreshAsync(string refreshToken)
        {
            var storedToken = await _repo.GetRefreshTokenAsync(refreshToken);

            if (storedToken == null)
                throw new Exception("Refresh token inválido");

            if (storedToken.IsRevoked)
                throw new Exception("Refresh token revogado");

            if (storedToken.Expires < DateTime.UtcNow)
                throw new Exception("Refresh token expirado");

            var user = storedToken.User;

            storedToken.IsRevoked = true;

            var newAccessToken = _tokenService.GenerateToken(user.Username, user.Role);

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var newRefreshEntity = new UserRefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _repo.SaveRefreshTokenAsync(newRefreshEntity);

            return new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
