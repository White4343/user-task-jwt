using FluentAssertions;
using UserTaskJWT.Web.Api.PasswordHashing;

namespace UserTaskJWT.UnitTests.UserTests
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void Hash_ShouldGenerateHashWithSalt()
        {
            // Arrange
            const string password = "testPassword";

            // Act
            string hashedPassword = _passwordHasher.Hash(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().Contain("-"); // Ensure it has the hash-salt format
        }

        [Fact]
        public void Hash_ShouldGenerateDifferentHashesForSamePassword()
        {
            // Arrange
            const string password = "testPassword";

            // Act
            string hash1 = _passwordHasher.Hash(password);
            string hash2 = _passwordHasher.Hash(password);

            // Assert
            hash1.Should().NotBe(hash2); // Due to random salt
        }

        [Fact]
        public void Verify_ShouldReturnTrueForCorrectPassword()
        {
            // Arrange
            const string password = "testPassword";
            string hashedPassword = _passwordHasher.Hash(password);

            // Act
            bool result = _passwordHasher.Verify(password, hashedPassword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_ShouldReturnFalseForIncorrectPassword()
        {
            // Arrange
            const string correctPassword = "testPassword";
            const string incorrectPassword = "wrongPassword";
            string hashedPassword = _passwordHasher.Hash(correctPassword);

            // Act
            bool result = _passwordHasher.Verify(incorrectPassword, hashedPassword);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Verify_ShouldThrowArgumentNullExceptionForNullPassword()
        {
            // Arrange
            string hashedPassword = _passwordHasher.Hash("somePassword");

            // Act
            Action act = () => _passwordHasher.Verify(null!, hashedPassword);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'password')");
        }

        [Fact]
        public void Verify_ShouldThrowArgumentNullExceptionForNullPasswordHash()
        {
            // Arrange
            const string password = "testPassword";

            // Act
            Action act = () => _passwordHasher.Verify(password, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'passwordHash')");
        }
    }
}