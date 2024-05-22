using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace TODOAPP.Helpers
{
  public class AuthHelpers
  {
    private readonly IConfiguration _config;
    public AuthHelpers(IConfiguration config)
    {
      _config = config;
    }
    public string GenerateToken(int userId)
    {
      Claim[] claims = new Claim[]{
        new Claim("userId",userId.ToString())
      };
      SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetSection("Jwt:Key").Value!));

      SigningCredentials credentials = new SigningCredentials(tokenKey,SecurityAlgorithms.HmacSha256);

      SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor(){
        Subject = new ClaimsIdentity(claims),
        SigningCredentials = credentials,
        Expires = DateTime.Now.AddHours(1)
      };
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      SecurityToken token = tokenHandler.CreateToken(descriptor);
      return tokenHandler.WriteToken(token);
    }
    public byte[] GeneratePasswordSalt()
    {
      byte[] passwordSalt = new byte[32];
      using(RandomNumberGenerator rng = RandomNumberGenerator.Create()){
        rng.GetNonZeroBytes(passwordSalt);
      }
      return passwordSalt;
    }
    public byte[] HashPassword(string plainPassword,byte[] passwordSalt )
    {
      byte[] passwordHash = KeyDerivation.Pbkdf2(
        password : plainPassword,
        salt:Encoding.ASCII.GetBytes(_config.GetSection("AdditionalSaltKey").Value+ Convert.ToBase64String(passwordSalt)),
        prf:KeyDerivationPrf.HMACSHA512,
        iterationCount:100000,
        numBytesRequested:32
      );
      return passwordHash;
    }
  }
}