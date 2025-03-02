using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductsAPI.Models;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options=>{
options.AddPolicy(name: MyAllowSpecificOrigins,
    policy=>{
               policy.WithOrigins("http://127.0.0.1:5500", "https://127.0.0.1:5500") // Frontend adresi
                  .AllowAnyMethod()
                  .AllowAnyHeader();
             
});
});

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

builder.Services.AddAuthentication(x=>{
    x.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
     x.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x=>{
    x.RequireHttpsMetadata=false;
    x.TokenValidationParameters=new TokenValidationParameters
    {
       ValidateIssuer= false,
       ValidIssuer="serhat.com", //firma
       ValidateAudience= false, //bu apiyi hangi firmalar ve servisler için geliştrtidn bilgisi
        ValidAudience="",
        ValidAudiences=new string[]{"a","b"}, //kimler için geliştirilmiş //false olanlar bilgilendirme truye çekmedim

         // benım kullandığım key bilgisi valudate edicez 
        ValidateIssuerSigningKey=true,
        IssuerSigningKey =new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:Secret").Value ?? "")),

        //süre için bilgi vermiştik ancak true verilemzse süre önemsemeden validate eder
        ValidateLifetime=true
    };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

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
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization(); // Eğer authentication gereksinimi varsa.

app.MapControllers(); // Bu satır çok önemli! Controller'ları tanımlamak için.
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
