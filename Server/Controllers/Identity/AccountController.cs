using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ForumSPA.Server.Authorization;
using ForumSPA.Server.Data.Models;
using ForumSPA.Shared.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ForumSPA.Server.Controllers.Identity
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var newUser = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new RegisterResult() { Succeeded = false, Errors = errors });
            }

            await _userManager.AddToRoleAsync(newUser, ForumConstants.UserRole);

            if (newUser.UserName.Equals(_configuration["Forum:AdminUsername"]))
                await _userManager.AddToRoleAsync(newUser, ForumConstants.AdministratorRole);

            return Ok(new RegisterResult() { Succeeded = true });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, false);

            if (!result.Succeeded)
            {
                return BadRequest(new LoginResult() { Succeeded = false, Error = "Username or password is invalid." });
            }

            var user = await _userManager.FindByNameAsync(login.Username);
            var roles = await _userManager.GetRolesAsync(user);

            // We want to add all the roles to our token
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, login.Username));

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtAudience"],
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            var secureToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResult() { Succeeded = true, Token = secureToken });
        }
    }
}