

using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.DTO;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private UserManager<AppUser>  _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration  _configuration;

        public UsersController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        /// <summary>
        /// Kullanıcı kayıt işlemi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(UserDto model)
        {
            if(!ModelState.IsValid)
            {
            return BadRequest(ModelState); 
            }
            
             var user= new AppUser{
                UserName=model.UserName,
                Email=model.Email,
                FullName=model.FullName,
                DateAdded=DateTime.Now
           };
           var result= await _userManager.CreateAsync(user,model.Password);
           if(result.Succeeded)
           {
            //return StatusCode(201); 201 durum kodu kaynak oluşturuldu bilinir
               return Ok(new{message="kullanıcı başarıyla oluşturuldu",user});
           }
         
           return BadRequest(result.Errors); //400lü hata kodu
       
    }
    /// <summary>
    /// Kullanıcı giriş (LOGİN) işlemi
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
     [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var user= await _userManager.FindByEmailAsync(model.Email);
        if(user==null)
        {
            return BadRequest(new{message="Email Hatalı Tekrar Deneyiniz"});
        }
        var result=  await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);
        if(result.Succeeded)
        {
            return Ok(
                new{token=GenerateJWT(user) } 
        );  //benzersiz bir token oluşturulacak verileri şifreleyip saklamak      isterse web isterse mobilde kullanılabilir 
        };
        return Unauthorized(new{message="Şifre Hatalı Tekrar Deneyiniz"});
    }

        private object GenerateJWT(AppUser user)
        {
           var tokenHandler= new JwtSecurityTokenHandler();
           var key=  Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? "");

           var tokenDescripter=new SecurityTokenDescriptor
           {
               Subject=new ClaimsIdentity(
                new Claim[]
                 {
                //token içerisinde saklanacak bilgiler
                   new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                   new Claim(ClaimTypes.Name,user.UserName ?? "")
               }
               ),
               Expires=DateTime.UtcNow.AddDays(1),
               //şifreleme algoritması
               SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha512Signature)
           };
           var token=tokenHandler.CreateToken(tokenDescripter);
           return tokenHandler.WriteToken(token);

        }
    }

}