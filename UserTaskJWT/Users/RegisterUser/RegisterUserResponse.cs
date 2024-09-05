namespace UserTaskJWT.Web.Api.Users.RegisterUser
{
    public record RegisterUserResponse(Guid Id, string Username, string Email, DateTime CreatedAt, DateTime UpdatedAt);
}