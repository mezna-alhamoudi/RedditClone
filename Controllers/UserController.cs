using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RedditClone.Models;
using RedditClone.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        // Find the user by their username
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null)
            return Unauthorized("Invalid username or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (!result.Succeeded)
            return Unauthorized("Invalid username or password");

        // If validation is successful, generate a JWT token as you were doing
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:SecretKey"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Name, model.UserName)
                // Add other claims as needed
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Return the token
        return Ok(new { Token = tokenString });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("User registered successfully");
            }

            return BadRequest(result.Errors);
        }

        return BadRequest("Invalid data");
    }
}



