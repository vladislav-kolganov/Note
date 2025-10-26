using Note.Domain.Dto;
using Note.Domain.Result;
using System.Security.Claims;

namespace Note.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExipredToken(string accessToken);
    Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);
}
