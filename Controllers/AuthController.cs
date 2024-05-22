using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TODOAPP.Data;
using TODOAPP.Dtos;
using TODOAPP.Helpers;
using TODOAPP.Models;

namespace TODOAPP.Controlers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class AuthController : ControllerBase
  {
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;
    private readonly AuthHelpers _helper;
    public AuthController(IConfiguration config, DataContextDapper dapper)
    {
      _dapper = dapper;
      _config = config;
      _helper = new AuthHelpers(config);
    }
    [HttpGet("Me")]
    public async Task<IActionResult> GetMe()
    {
      string? userId = this.User.FindFirstValue("userId");
      User? user = await _dapper.LoadSingleData<User>("SELECT * FROM TodoAppSchema.[User] WHERE UserId=@UserId",
        new DynamicParameters(new { UserId = userId })
      );
      return Ok(user);
    }
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
      // Check if user exists 
      User? user = await _dapper.LoadSingleData<User>("SELECT * FROM TodoAppSchema.[User] WHERE Email=@Email",
        new DynamicParameters(new { Email = loginUserDto.Email })
      );
      if (user == null) return BadRequest(new { Message = "Email or password is invalid" });
      // check if password is valid
      UserAuthDto? userAuth = await _dapper.LoadSingleData<UserAuthDto>("SELECT [PasswordSalt],[PasswordHash] FROM TodoAppSchema.[Auth] WHERE UserId=@UserId", new DynamicParameters(new { UserId = user.UserId }));
      if (userAuth == null) return BadRequest(new { Message = "Email or password is invalid" });
      // Get Hashed Password
      byte[] passwordHash = _helper.HashPassword(loginUserDto.Password, userAuth.PasswordSalt);
      // Comapre the two
      for (int i = 0; i < passwordHash.Length; i++)
      {
        if (passwordHash[i] != userAuth.PasswordHash[i])
        {
          return Unauthorized(new { Message = "Email or password is invalid" });
        }
      }
      // Create Token
      string token = _helper.GenerateToken(user.UserId);
      return Ok(new { Token = token });
    }
    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
      // Check if email exists
      int emailExists = await _dapper.LoadSingleData<int>("SELECT COUNT(*) FROM TodoAppSchema.[User] WHERE Email=@Email",
        new DynamicParameters(new { Email = registerUserDto.Email })
      );
      if (emailExists > 0)
        return BadRequest(new { Mmessage = "Email already taken" });
      // Create User
      bool UserCreated = await _dapper.ExecuteSql("INSERT INTO TodoAppSchema.[User]([Email],[FullName]) VALUES(@Email,@FullName)",
      new DynamicParameters(
        new { Email = registerUserDto.Email, FullName = registerUserDto.FullName }
      ));
      if (!UserCreated) return StatusCode(500, new { Message = "User count not be created" });
      // Hash Password 
      // 1 create salt
      byte[] passwordSalt = _helper.GeneratePasswordSalt();
      // 2 Crate Hash 
      byte[] passwordHash = _helper.HashPassword(registerUserDto.Password, passwordSalt);
      // Get UserId
      int userId = await _dapper.LoadSingleData<int>("SELECT UserId FROM TodoAppSchema.[User] WHERE Email=@Email",
        new DynamicParameters(new { Email = registerUserDto.Email })
      );
      // Create Auth
      await _dapper.ExecuteSql("INSERT INTO TodoAppSchema.[Auth]([UserId],[PasswordSalt],[PasswordHash]) VALUES(@UserId,  @PasswordSalt,@PasswordHash)", new DynamicParameters(new { UserId = userId, PasswordSalt = passwordSalt, PasswordHash = passwordHash })
      );
      // Create Token
      string token = _helper.GenerateToken(userId);
      return Ok(new { Token = token });
    }
  }
}