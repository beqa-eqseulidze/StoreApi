
namespace Application.TDOs
{
    public record AuthResponseDto(string AccessToken, DateTimeOffset RefreshTokenExpiration);
}
