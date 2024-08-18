using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZWalks.API.Mappings;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.Domain.RepositoryInterface;
using NZWalks.Core.Services;
using NZWalks.Core.ServicesInterface;
using NZWalks.Infrastructure.DataContext;
using NZWalks.Infrastructure.Repositories;
using Serilog;
using NZWalks.API.Middleware;
using NZWalks.API;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 9, Add Serilog 
var logger = new LoggerConfiguration()
    .WriteTo.Console() // Define where to write Log (here is Console)
    .MinimumLevel.Information() // or: Error(), Warning(), Debug(),...
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger); // Add Serilog

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Keycloack",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.OpenIdConnect,
        //OpenIdConnectUrl = new Uri($"{builder.Configuration["Keycloak:auth-server-url"]}realms/{builder.Configuration["Keycloak:realm"]}/.well-known/openid-configuration"),
        OpenIdConnectUrl = new Uri($"http://localhost:8080/realms/aspnet/.well-known/openid-configuration"),
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, Array.Empty<string>()}
    });


});

// 1, Inject DBContext, Entity Framework and provide the connection string to DBContext
var server = builder.Configuration["server"] ?? "localhost";
var database = builder.Configuration["database"] ?? "NZWalksDb";
var port = builder.Configuration["port"] ?? "5432";
var password = builder.Configuration["password"] ?? "1234567890";
var user = builder.Configuration["dbUser"] ?? "postgres";

var connectionString = $"User ID={user};Host={server};Port={port};Database={database};Username={user};Password={password}";

//builder.Services.AddDbContext<NZWalksDbContext>(options =>
//options.UseNpgsql(builder.Configuration.GetConnectionString("NZWalksConnectionStringPosgresSQL")));
builder.Services.AddDbContext<NZWalksDbContext>(options =>
options.UseNpgsql(connectionString));

// 6, Inject new DBContext, Entity Framework (for authenticate and authorize) and provide the connection string to DBContext
// có thể tạo DB mới dành cho việc lưu trữ User Authenticated này bằng cách tạo một ConnectionString mới
// trong appsettings.json rồi thay biến đó vào dưới
builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
options.UseNpgsql(connectionString));

// 2, Inject Repositories and Services in Ioc Container (Scoped Service)    
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IWalkRepository, WalkRepository>();

builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IWalkService, WalkService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 3, Inject AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// 7, Setup Identity 
builder.Services
    .AddIdentityCore<User>() // Thay IdentityUser thành User (đc extend từ IdentityUser)
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>("NZWalks")
    .AddEntityFrameworkStores<NZWalksAuthDbContext>()
    .AddDefaultTokenProviders();

// 8, Config password
builder.Services.Configure<IdentityOptions>(option =>
{
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
    option.Password.RequiredLength = 6;
    option.Password.RequiredUniqueChars = 1;
});

// 4, Add authentication
// Default authentication schemas
builder.Services
    //.AddAuthorization(o =>
    //{
    //    o.DefaultPolicy = new AuthorizationPolicyBuilder()
    //        .RequireAuthenticatedUser()
    //        .RequireClaim("email_verified", "true")
    //        .Build();
    //})
    .AddAuthentication(options =>
    {
        //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
{
    x.Authority = $"http://localhost:8080/realms/aspnet";
    x.RequireHttpsMetadata = false; // Nếu chạy ở môi trường development
    x.SaveToken = true;
    x.Audience = "account";
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "http://localhost:8080/realms/aspnet", // Đảm bảo rằng giá trị này khớp với "iss" trong JWT
        ValidateAudience = true,
        ValidAudience = "account", // must define this
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        //RoleClaimType = "roles"
    };
    

});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()    // Allows requests from any origin
            .AllowAnyHeader()    // Allows any headers
            .AllowAnyMethod();     // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
    });
});


var app = builder.Build();


DatabaseManagementService.MigrationInitialisation(app);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//{
// ko nên bỏ comment condition này (vì demo docker nên đành vậy)
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }

    await next();
});

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var claims = context.User.Claims;
        foreach (var claim in claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }
    }
    await next();
});

app.UseRouting();

// 11, Disabled CORS
app.UseCors("AllowSpecificOrigins");

// 12, Đăng ký middleware tùy chỉnh trước khi UseAuthentication và UseAuthorization
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<CustomAuthorizationMiddleware>();


// 5, Trước khi dùng app.UseAuthorization() thì phải app.UseAuthentication() trước
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
