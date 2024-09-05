namespace UserTaskJWT.Web.Api.Users
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}