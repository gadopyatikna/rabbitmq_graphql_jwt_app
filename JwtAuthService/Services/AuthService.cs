using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthService.Models;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthService.Services;

public class AuthService
{
    private readonly List<User> _users = new(); // In-memory user store

    private readonly string _key;

    public AuthService(string key)
    {
        _key = key;
    }

    public void RegisterUser(User user)
    {
        _users.Add(user);
    }

    public string Authenticate(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}