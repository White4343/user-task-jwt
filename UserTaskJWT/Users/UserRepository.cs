using Microsoft.EntityFrameworkCore;
using UserTaskJWT.Web.Api.Data;

namespace UserTaskJWT.Web.Api.Users
{
    public class UserRepository(AppDbContext context, ILogger<UserRepository> logger) : IUserRepository
    {
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(context.Users);
                ArgumentNullException.ThrowIfNull(user);

                var createdUser = await context.Users.AddAsync(user, 
                    cancellationToken).ConfigureAwait(false);
                
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                logger.LogInformation("User {Username} created", user.Username);

                return createdUser.Entity;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(context.Users);

                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                logger.LogInformation("User {Username} found", username);

                return user;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(context.Users);

                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                logger.LogInformation("User {Email} found", email);

                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}