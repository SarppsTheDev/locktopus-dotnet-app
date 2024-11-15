using Microsoft.AspNetCore.Identity;
using passwordvault_dataaccess;
using passwordvault_domain;
using passwordvault_domain.Entities;
using passwordvault_domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// User Authentication
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

// Database Context
builder.Services.AddDbContext<AppDbContext>();

// App Services
builder.Services.AddTransient<ILoginItemService, LoginItemService>();

// Register the Identity services
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

app.MapIdentityApi<User>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();