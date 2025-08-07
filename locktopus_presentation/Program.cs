using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using locktopus_dataaccess;
using locktopus_dataaccess.Repositories;
using locktopus_domain.Entities;
using locktopus_domain.Helpers;
using locktopus_domain.Repositories;
using locktopus_domain.Services;

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
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserQueryRepository, UserQueryRepository>();

// Domain Services
builder.Services.AddScoped<IUserContextHelper, UserContextHelper>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<ILoginItemService, LoginItemService>();
builder.Services.AddTransient<IUserService, UserService>();

// Register the Identity services
builder.Services.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints()
    .AddDefaultTokenProviders();

// Set Password Reset Token Lifespan
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromMinutes(15));

var app = builder.Build();

app.MapIdentityApi<User>();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigins"); 

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();