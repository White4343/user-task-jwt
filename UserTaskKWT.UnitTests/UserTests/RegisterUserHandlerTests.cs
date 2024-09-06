using NSubstitute;
using FluentAssertions;
using FluentValidation;
using UserTaskJWT.Web.Api.PasswordHashing;
using UserTaskJWT.Web.Api.Users;
using UserTaskJWT.Web.Api.Users.RegisterUser;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace UserTaskJWT.UnitTests.UserTests
{
    public class RegisterUserHandlerTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly RegisterUserValidator _registerUserValidator = Substitute.For<RegisterUserValidator>();
        private readonly RegisterUserHandler _handler;

        public RegisterUserHandlerTests()
        {
            _handler = new RegisterUserHandler(_userRepository, _passwordHasher, _registerUserValidator);
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_RegistersUserAndReturnsResponse()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
            var validationResult = new ValidationResult(); // Valid result
            _registerUserValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);
            _passwordHasher.Hash(command.Password).Returns("hashed_password");

            User user = new User
            {
                Id = Guid.NewGuid(),
                Username = command.Username,
                Email = command.Email,
                PasswordHash = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().NotBeEmpty();
            response.Username.Should().Be(command.Username);
            response.Email.Should().Be(command.Email);
            response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            response.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            ArgumentNullException.ThrowIfNull(response);

            await _userRepository.Received(1).AddAsync(user, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_NullCommand_ThrowsArgumentNullException()
        {
            // Arrange
            RegisterUserCommand? command = new RegisterUserCommand("", "", "");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_InvalidCommand_ThrowsValidationException()
        {
            // Arrange
            var command = new RegisterUserCommand("invalid", "invalid", "invalid");
            var validationResult = new ValidationResult(new[] { new ValidationFailure("Property", "Error") }); // Invalid result
            _registerUserValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_EmailTaken_ThrowsBadHttpRequestException()
        {
            // Arrange
            var command = new RegisterUserCommand("testuser", "taken@example.com", "password123");
            var validationResult = new ValidationResult(); // Valid result
            _registerUserValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UsernameTaken_ThrowsBadHttpRequestException()
        {
            // Arrange
            var command = new RegisterUserCommand("takenuser", "test@example.com", "password123");
            var validationResult = new ValidationResult(); // Valid result
            _registerUserValidator.ValidateAsync(command, Arg.Any<CancellationToken>()).Returns(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }
    }
}