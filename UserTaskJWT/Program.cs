var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/love", () => "Hello World!").WithOpenApi();

app.UseHttpsRedirection();

await app.RunAsync().ConfigureAwait(false);