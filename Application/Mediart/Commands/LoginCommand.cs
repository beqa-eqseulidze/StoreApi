
using Application.Common.Interfaces;
using Application.Interfaces;
using Application.TDOs;
using FluentValidation;
using MediatR;

namespace Application.Mediart.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;

    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }

    public sealed class LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Simulate user retrieval from a data source
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshTokenExpiration = DateTimeOffset.UtcNow.AddDays(7); // Example expiration time
            return new AuthResponseDto(accessToken, refreshToken, refreshTokenExpiration);
        }
       
    }
}
