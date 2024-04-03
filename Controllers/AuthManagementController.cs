using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TodoAPP.Configuration;
using TodoAPP.Models.DTOs.Requests;
using TodoAPP.Models.DTOs.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace TodoAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfiguration _jwtConfiguration;

        public AuthManagementController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfiguration> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfiguration = optionsMonitor.CurrentValue;
        }




        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegistationDTO user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return BadRequest(new RegistationResponse
                    {
                        Errors = new List<string> { "Email already in use" },
                        Success = false
                    });
                }

                var newUser = new IdentityUser
                {
                    Email = user.Email,
                    UserName = user.UserName
                };

                var isCreated = await _userManager.CreateAsync(newUser, user.Password);

                if (!isCreated.Succeeded)
                {
                    return BadRequest(new RegistationResponse
                    {
                        Errors = isCreated.Errors.Select(error => error.Description).ToList(),
                        Success = false
                    });
                }
                else
                {
                   var jwtToken =  GenerateJwtToken(newUser);
                   return Ok (new RegistationResponse()
                   {
                     Success=true,
                     Token=jwtToken,
                   });
                }
            }

            return BadRequest(new RegistationResponse
            {
                Errors = new List<string> { "Invalid request" },
                Success = false
            });
        }

        private string  GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);
            var  tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject  = new ClaimsIdentity(new[]
                 { 
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                 }),
                 Expires=DateTime.UtcNow.AddSeconds(10),
                 SigningCredentials = new SigningCredentials(new  SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

             };
              var token = jwtTokenHandler.CreateToken(tokenDescriptor);
              var jwtToken = jwtTokenHandler.WriteToken(token);
              return jwtToken;
        }

        [HttpPost(Name ="Login")]
        public async Task <ActionResult > Login([FromBody] UserLoginRequest user)
        {
                if(ModelState.IsValid)
                {
                    var existingUser = await  _userManager.FindByEmailAsync(user.Email);

                    if (existingUser == null){
                         return BadRequest(new RegistationResponse()
                            {
                                Errors = new List<string> { "Invalid Login request" },
                                Success = false
                            });
                }
               var IsCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);
               if (!IsCorrect)
               {
                   return BadRequest(new RegistationResponse()
                            {
                                Errors = new List<string> { "Invalid Login request" },
                                Success = false
                            });
               }
               var jwtToken= GenerateJwtToken(existingUser);
                return Ok(
                    new RegistationResponse(){
                        Success=true,
                        Token = jwtToken

                    });
            }
            return BadRequest(new RegistationResponse()
            {
                Errors = new List<string> { "Invalid request" },
                Success = false
            });
        }
    }
}
