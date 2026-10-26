namespace Application.TDOs.Auth
{
    public record AuthResponseDto(string JwtToken, DateTimeOffset TokenExpiration);
}
