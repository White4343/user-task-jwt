namespace UserTaskJWT.Web.Api.Data
{
    public static class DbInitialize
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            await context.Database.EnsureCreatedAsync().ConfigureAwait(false);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}