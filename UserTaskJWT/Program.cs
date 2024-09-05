using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserTaskJWT.Web.Api.Data;
using UserTaskJWT.Web.Api.Endpoints;
using UserTaskJWT.Web.Api.Middleware;
using UserTaskJWT.Web.Api.PasswordHashing;
using UserTaskJWT.Web.Api.Users;
using UserTaskJWT.Web.Api.Users.RegisterUser;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DockerConnectionString")));

builder.Services.AddEndpoints();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<RegisterUserHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapEndpoints();

app.MapGet("/love", () => "Hello World!").WithOpenApi();

app.UseHttpsRedirection();

CreateDbIfNotExists(app);

await app.RunAsync().ConfigureAwait(false);

void CreateDbIfNotExists(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        DbInitialize.InitializeAsync(context).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}