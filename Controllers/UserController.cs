using BudMath.Helpers;
using BudMath.Models;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service.Implementation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudMath.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IConfiguration _Configuration;
        public UserController(IConfiguration configuration, UserManager<ApplicationUser> UserManager)
        {
            _UserManager = UserManager;
            _Configuration = configuration;
            
        }
        [HttpPost]
        [Route("Login")]
        //api/User/Login
        public async Task<IActionResult> SigninUser([FromBody] Login _Users)
        {
            var user = await _UserManager.FindByNameAsync(_Users.Username);
            if (user != null && await _UserManager.CheckPasswordAsync(user, _Users.Password))
            {
                var userRoles = await _UserManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role,userRole));
                }
                var TokenDetails = TokenHelper.GenerateToken(_Configuration, DateTime.Now.AddHours(3), authClaims);
                return Ok(TokenDetails);
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("Register")]
        //api/User/Register
        public async Task<IActionResult> AddNewUser([FromBody] Register _Users)
        {
            var userExists = _UserManager.FindByNameAsync(_Users.Username);
            if(userExists != null)
            {
                return StatusCode(StatusCodes.Status208AlreadyReported, new  { Status= "Error", Message="User Already Exists" });
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = _Users.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = _Users.Username
            };
            var result = await _UserManager.CreateAsync(user, _Users.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User Not Created" });
            }
            return Ok(new { Status = "Success", Message = "User Created Successfully"});
        }
    }
}
