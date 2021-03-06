using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApi.Data;
using DatingApi.Dtos;
using DatingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _config;

    public AuthController(IAuthRepository repository, IConfiguration config)
    {
      _repository = repository;
      _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
    {
      // validate request
      userForRegisterDto.username = userForRegisterDto.username.ToLower();

      if (await _repository.UserExists(userForRegisterDto.username))
      {
        return BadRequest("Username already exists");
      }

      var userToCreate = new User
      {
        Username = userForRegisterDto.username
      };

      var createdUser = await _repository.Register(userToCreate, userForRegisterDto.password);

      return StatusCode(201);

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
      
      var userFromRepo = await _repository.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

      if (userFromRepo == null)
      {
          return Unauthorized();
      }

      var claims = new[]
      {
        new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
        new Claim(ClaimTypes.Name, userFromRepo.Username)
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor); 

      return Ok(new {
        token = tokenHandler.WriteToken(token)
      });
    }
  }
}