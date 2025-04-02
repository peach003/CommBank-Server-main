using CommBank.Models;
using CommBank.Services;
using MongoDB.Driver;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ensure Secrets.json is loaded
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Secrets.json", optional: true, reloadOnChange: true);

// Get MongoDB connection string from Secrets.json
var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("CommBank"));
var mongoDatabase = mongoClient.GetDatabase("CommBank");

// Register services
builder.Services.AddSingleton<IAccountsService>(new AccountsService(mongoDatabase));
builder.Services.AddSingleton<IAuthService>(new AuthService(mongoDatabase));
builder.Services.AddSingleton<IGoalsService>(new GoalsService(mongoDatabase));
builder.Services.AddSingleton<ITagsService>(new TagsService(mongoDatabase));
builder.Services.AddSingleton<ITransactionsService>(new TransactionsService(mongoDatabase));
builder.Services.AddSingleton<IUsersService>(new UsersService(mongoDatabase));

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// CORS setup to allow all origins, methods, and headers
app.UseCors("AllowAllOrigins");

// Configure Swagger for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authorization middleware
app.UseAuthorization();

// Map controllers to the app
app.MapControllers();

// Log request details for debugging
app.Use(async (context, next) =>
{
    // Log request method and URL
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

// Run the application
app.Run();

