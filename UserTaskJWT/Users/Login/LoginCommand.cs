namespace UserTaskJWT.Web.Api.Users.Login
{
    public record LoginCommand(string? Email, string? Username, string Password);
}