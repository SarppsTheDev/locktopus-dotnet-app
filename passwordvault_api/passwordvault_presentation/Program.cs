using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using passwordvault_dataaccess;
using passwordvault_dataaccess.Repositories;
using passwordvault_domain.Entities;
using passwordvault_domain.Helpers;
using passwordvault_domain.Repositories;
using passwordvault_domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:8080") // Replace with your frontend's origin
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Password Vault API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// User Authentication
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

// Database Context
builder.Services.AddDbContext<AppDbContext>();

// Data Repositories
builder.Services.AddTransient<ILoginItemRepository, LoginItemRepository>();
builder.Services.AddTransient<ILoginItemQueryRepository, LoginItemQueryRepository>();

// Domain Services
builder.Services.AddScoped<IUserContextHelper, UserContextHelper>();
builder.Services.AddTransient<ILoginItemService, LoginItemService>();

// Register the Identity services
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

app.MapIdentityApi<User>();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigins"); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();