using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Models;

var builder = WebApplication.CreateBuilder(args);
// Swagger için XML dokümantasyonu ekleyelim
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(xmlPath);
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ProductsContext>(options =>
    options.UseSqlServer("Server=SERHAT\\SQLEXPRESS;Database=ProductsDB;Trusted_Connection=True;TrustServerCertificate=True;"));
builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<ProductsContext>();
builder.Services.Configure<IdentityOptions>(options =>{
    options.Password.RequiredLength = 6;
    //alt çizgi @ gibi işratler zorunlu değil
    options.Password.RequireNonAlphanumeric=false;
    //bütyük küçük harf zorunluğu yok
    options.Password.RequireLowercase=false;
    options.Password.RequireUppercase=false;
    options.Password.RequireDigit=false;
    options.User.RequireUniqueEmail=true;
    options.User.AllowedUserNameCharacters="abcçdefgğhıijklmnoöprsştuüvyzABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ0123456789-._@+";
    // yanlış girişten sonra 5dk bekle
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5);
   // 5kere yanlış giriş sonrası kullanıucıyı kilitleme
    options.Lockout.MaxFailedAccessAttempts=5;



});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
app.UseAuthorization(); // Eğer authentication gereksinimi varsa.

app.MapControllers(); // Bu satır çok önemli! Controller'ları tanımlamak için.
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
