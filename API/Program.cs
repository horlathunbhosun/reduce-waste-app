using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    var mysqlServerVersion =  ServerVersion.AutoDetect(connectionString);
    options.UseMySql(connectionString, mysqlServerVersion);
});

//uncomment if you are using SqlServer

// builder.Services.AddDbContext<ApplicationDbContext>(options => {
//     var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
//     var mysqlServerVersion =  ServerVersion.AutoDetect(connectionString);
//     options.UseMySql(connectionString, mysqlServerVersion);
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();
app.Run();
