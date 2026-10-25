using Application.TDOs;
using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Mediart.Commands
{
    public record RegisterCommand( string FirstName, string LastName, string Email, string Password) : IRequest<AuthResponseDto>;

    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }

    public sealed class RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if the email is already registered
            var existingUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already registered.");
            }
            // Hash the password
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            // Create a new user entity
            var newUser = new Domain.Entities.User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = Domain.Enums.UserRole.Customer // Default role for new users
            };
            // Add the new user to the database
            await _unitOfWork.UserRepository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // Generate JWT tokens
            var jwtToken = _jwtTokenGenerator.GenerateAccessToken(newUser);
            var tokenExpiration = DateTimeOffset.UtcNow.AddDays(1); // Example expiration time
            return new AuthResponseDto(jwtToken, tokenExpiration);
        }
    }
}
