using CommBank.Models;
using CommBank.Services;
using MongoDB.Driver;
using System.Net;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Secrets.json", optional: true, reloadOnChange: true);

var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("CommBank"));
var mongoDatabase = mongoClient.GetDatabase("CommBank");


builder.Services.AddSingleton<IAccountsService>(new AccountsService(mongoDatabase));
builder.Services.AddSingleton<IAuthService>(new AuthService(mongoDatabase));
builder.Services.AddSingleton<IGoalsService>(new GoalsService(mongoDatabase));
builder.Services.AddSingleton<ITagsService>(new TagsService(mongoDatabase));
builder.Services.AddSingleton<ITransactionsService>(new TransactionsService(mongoDatabase));
builder.Services.AddSingleton<IUsersService>(new UsersService(mongoDatabase));

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


app.UseCors("AllowAllOrigins");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.UseAuthorization();


app.MapControllers();


app.Use(async (context, next) =>
{
    
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});


app.Run();

