namespace UserTaskJWT.Web.Api.Users.RegisterUser
{
    public record RegisterUserCommand(string Username, string Email, string Password);
}