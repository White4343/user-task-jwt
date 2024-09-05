using FluentValidation;
using UserTaskJWT.Web.Api.PasswordHashing;
using BadHttpRequestException = Microsoft.AspNetCore.Http.BadHttpRequestException;

namespace UserTaskJWT.Web.Api.Users.RegisterUser
{
    public class RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IValidator<RegisterUserCommand> registerUserValidator)
    {
        public async Task<RegisterUserResponse> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);

            // validate password, email, username
            var validationResult = await registerUserValidator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // check if email is already taken
            if (await IsEmailTaken(command.Email, cancellationToken).ConfigureAwait(false))
            {
                throw new BadHttpRequestException("The email is already taken");
            }

            // check if username is already taken
            if (await IsUsernameTaken(command.Username, cancellationToken).ConfigureAwait(false))
            {
                throw new BadHttpRequestException("The username is already taken");
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                Username = command.Username,
                Email = command.Email,
                PasswordHash = passwordHasher.Hash(command.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);

            var response = new RegisterUserResponse(user.Id, user.Username, user.Email, user.CreatedAt, user.UpdatedAt);

            return response;
        }

        private async Task<bool> IsEmailTaken(string email, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken).ConfigureAwait(false);

            return user != null;
        }

        private async Task<bool> IsUsernameTaken(string username, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByUsernameAsync(username, cancellationToken).ConfigureAwait(false);

            return user != null;
        }
    }
}