using API.Data;
using API.Dtos.Response;
using API.Models;
using API.Repositories;
using API.Repositories.Interface;
using API.Services.UserService;
using API.Services.Email;
using API.Services.MagicBag;
using API.Services.Product;
using API.Services.Token;
using API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()  
    .AddJsonOptions(options =>
{
   // options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
   // options.JsonSerializerOptions.WriteIndented = true; // Optional for pretty formatting

    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = false;
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    var mysqlServerVersion =  ServerVersion.AutoDetect(connectionString);
    options.UseMySql(connectionString, mysqlServerVersion);
});
//uncomment if you are using SqlServe
// builder.Services.AddDbContext<ApplicationDbContext>(options => {
//     var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
//     var mysqlServerVersion =  ServerVersion.AutoDetect(connectionString);
//     options.UseMySql(connectionString, mysqlServerVersion);
// });


builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    // options.User.RequireUniqueEmail = true;
    // options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    // options.SignIn.RequireConfirmedEmail = true;


}).AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = 
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = 
                    options.DefaultSignInScheme =
                        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                    
    
}).AddJwtBearer( options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!))
        
        
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            GenericResponse response = GenericResponse.FromError(new ErrorResponse("Invalid Token", "Unauthorized", StatusCodes.Status401Unauthorized), StatusCodes.Status401Unauthorized);
            return context.Response.WriteAsJsonAsync(response);
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
             GenericResponse response = GenericResponse.FromError(new ErrorResponse("Unauthorized", "Unauthorized", StatusCodes.Status401Unauthorized), StatusCodes.Status401Unauthorized);          
             return context.Response.WriteAsJsonAsync(response);
        },  
        OnForbidden = context =>
        {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        GenericResponse response = GenericResponse.FromError(new ErrorResponse("Forbidden", "You do not have permission to access this resource", StatusCodes.Status403Forbidden), StatusCodes.Status403Forbidden);
        return context.Response.WriteAsJsonAsync(response);
    }
        
    };
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IMagicBagRepository, MagicBagRepository>();
builder.Services.AddScoped<IMagicBagService, MagicBagService>();

builder.Services.AddScoped<IProductMagicBagItemRepository, ProductMagicBagItemRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy=> policy.RequireRole("Admin"));
    options.AddPolicy("User", policy=> policy.RequireRole("User"));
    options.AddPolicy("Partner", policy=> policy.RequireRole("Partner"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithOrigins("https://localhost:4200")
    .SetIsOriginAllowed(origin => true));

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();


app.Run();


//ownerproof-4357564-1733050997-ae15be166471