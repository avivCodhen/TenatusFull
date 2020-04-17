using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tenatus.API.Components.Authentication.Models;
using Tenatus.API.Data;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Tenatus.API.Components.Authentication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int TokenExpiredDays = 30;

        public AuthController(IConfiguration configuration, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> SignIn(UserLoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var results = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                if (!results.Succeeded)
                {
                    throw new Exception("Failed to sign in");
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Route("signup")]
        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignInModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                    return BadRequest("Email exists");
                var user = new ApplicationUser() {Email = model.Email, UserName = model.Email};
                var results = await _userManager.CreateAsync(user, model.Password);
                if (!results.Succeeded)
                {
                    throw new Exception($"Error: {results.Errors}");
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }


        /*
        [HttpPost]
        [Route("test")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Test()
        {
            return Ok();
        }
        */


        [Route("token")]
        public async Task<IActionResult> Token(UserLoginModel model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                
                var issuer = _configuration["Token:Issuer"];
                var audience = _configuration["Token:Audience"];
                var key = _configuration["Token:Key"];

                var results = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (results.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                    };
                    var keyBytes = Encoding.UTF8.GetBytes(key);
                    var theKey = new SymmetricSecurityKey(keyBytes);
                    var creds = new SigningCredentials(theKey, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(issuer, audience, claims,
                        expires: DateTime.Now.AddDays(TokenExpiredDays), signingCredentials: creds);

                    return Ok(new {token = new JwtSecurityTokenHandler().WriteToken(token)});
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}