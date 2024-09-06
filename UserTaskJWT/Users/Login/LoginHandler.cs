using UserTaskJWT.Web.Api.JwtProviderService;
using UserTaskJWT.Web.Api.PasswordHashing;

namespace UserTaskJWT.Web.Api.Users.Login
{
    public class LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        public async Task<string> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);

            // if both email and username are empty
            if (String.IsNullOrEmpty(command.Email) && String.IsNullOrEmpty(command.Username))
            {
                throw new BadHttpRequestException("Email or Username is required");
            }

            // if password is empty
            if (String.IsNullOrEmpty(command.Password))
            {
                throw new BadHttpRequestException("Password is required");
            }

            User? user = null;

            // get user by email or username
            if (!String.IsNullOrEmpty(command.Email))
            {
                user = await userRepository.GetByEmailAsync(command.Email, cancellationToken).ConfigureAwait(false);

                ArgumentNullException.ThrowIfNull(user);
            }
            // get user by username
            else if (!String.IsNullOrEmpty(command.Username))
            {
                user = await userRepository.GetByUsernameAsync(command.Username, cancellationToken).ConfigureAwait(false);

                ArgumentNullException.ThrowIfNull(user);
            }

            ArgumentNullException.ThrowIfNull(user);

            // verify password
            if (!passwordHasher.Verify(command.Password, user.PasswordHash))
            {
                throw new BadHttpRequestException("Invalid password");
            }

            string token = jwtProvider.Generate(user);

            return token;
        }
    }
}
