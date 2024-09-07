using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserTaskJWT.Web.Api.Data;
using UserTaskJWT.Web.Api.Endpoints;
using UserTaskJWT.Web.Api.JwtProviderService;
using UserTaskJWT.Web.Api.Middleware;
using UserTaskJWT.Web.Api.PasswordHashing;
using UserTaskJWT.Web.Api.Tasks;
using UserTaskJWT.Web.Api.Tasks.CreateTask;
using UserTaskJWT.Web.Api.Tasks.DeleteTask;
using UserTaskJWT.Web.Api.Tasks.GetTaskById;
using UserTaskJWT.Web.Api.Tasks.GetTasksByUserId;
using UserTaskJWT.Web.Api.Tasks.UpdateTask;
using UserTaskJWT.Web.Api.Users;
using UserTaskJWT.Web.Api.Users.Login;
using UserTaskJWT.Web.Api.Users.RegisterUser;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] 
                                       ?? throw new InvalidOperationException()))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DockerConnectionString"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddEndpoints();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator>();
builder.Services.AddScoped<IValidator<CreateTaskCommand>, CreateTaskValidator>();
builder.Services.AddScoped<IValidator<UpdateTaskCommand>, UpdateTaskValidator>();
builder.Services.AddScoped<IValidator<GetTasksByUserIdQuery>, GetTasksByUserIdValidator>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<CreateTaskHandler>();
builder.Services.AddScoped<GetTaskByIdHandler>();
builder.Services.AddScoped<UpdateTaskHandler>();
builder.Services.AddScoped<DeleteTaskHandler>();
builder.Services.AddScoped<GetTasksByUserIdHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapEndpoints();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

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