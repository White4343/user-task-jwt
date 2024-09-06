using FluentAssertions;
using NSubstitute;
using Microsoft.AspNetCore.Http;
using UserTaskJWT.Web.Api.JwtProviderService;
using UserTaskJWT.Web.Api.PasswordHashing;
using UserTaskJWT.Web.Api.Users;
using UserTaskJWT.Web.Api.Users.Login;

namespace UserTaskJWT.UnitTests.UserTests
{
    public class LoginHandlerTests
    {
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly IJwtProvider _jwtProvider = Substitute.For<IJwtProvider>();
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _handler = new LoginHandler(_userRepository, _passwordHasher, _jwtProvider);
        }

        [Fact]
        public async Task HandleAsync_ValidCommandWithEmail_ReturnsToken()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "null", "password123");
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                Username = "null",
                CreatedAt = default,
                UpdatedAt = default
            };

            ArgumentNullException.ThrowIfNull(command.Email);

            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
            _jwtProvider.Generate(user).Returns("generated_token");

            // Act
            var token = await _handler.HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            token.Should().Be("generated_token");
        }

        [Fact]
        public async Task HandleAsync_ValidCommandWithUsername_ReturnsToken()
        {
            // Arrange
            var command = new LoginCommand(null, "testuser", "password123");
            var user = new User
            {
                Username = "testuser",
                PasswordHash = "hashed_password",
                Email = "null",
                CreatedAt = default,
                UpdatedAt = default
            };

            ArgumentNullException.ThrowIfNull(command.Username);

            _userRepository.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
            _jwtProvider.Generate(user).Returns("generated_token");

            // Act
            var token = await _handler.HandleAsync(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            token.Should().Be("generated_token");
        }

        [Fact]
        public async Task HandleAsync_EmptyEmailAndUsername_ThrowsBadHttpRequestException()
        {
            // Arrange
            var command = new LoginCommand(null, null, "password123");

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_EmptyPassword_ThrowsBadHttpRequestException()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", null, "");

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UserNotFoundWithEmail_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new LoginCommand("nonexistent@example.com", null, "password123");

            ArgumentNullException.ThrowIfNull(command.Email);

            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_UserNotFoundWithUsername_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new LoginCommand(null, "nonexistentuser", "password123");

            ArgumentNullException.ThrowIfNull(command.Username);

            _userRepository.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }

        [Fact]
        public async Task HandleAsync_InvalidPassword_ThrowsBadHttpRequestException()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "null", "wrongpassword");
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                Username = "null",
                CreatedAt = default,
                UpdatedAt = default
            };

            ArgumentNullException.ThrowIfNull(command.Email);

            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.HandleAsync(command, CancellationToken.None)).ConfigureAwait(false);
        }
    }
}