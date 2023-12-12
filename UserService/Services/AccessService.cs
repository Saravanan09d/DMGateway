using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Data;
using UserService.Model.DTO;
using UserService.Models;
using UserService.Models.DTO;

namespace UserService.Services
{
    public class AccessService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AccessService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserDetailsDTO> AuthenticateAsync(string Email, string password)
        {
            var user = await _context.UserTableModel.FirstOrDefaultAsync(u => u.Email == Email);

            if (user != null)
            {
                if (VerifyPassword(user.Password, password))
                {
                    var role = await _context.UserRoleModel.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                    var userDetailsDTO = new UserDetailsDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        RoleId = user.RoleId,
                        RoleName = role?.RoleName,
                        Token = GenerateJwtToken(user)
                    };
                    return userDetailsDTO;
                }
            }
            return null;
        }


        public string GenerateJwtToken(UserTableModelDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Id.ToString()),
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string storedHashedPassword, string enteredPassword)
        {
            var passwordHasher = new PasswordHasher<UserTableModelDTO>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, storedHashedPassword, enteredPassword);
            return passwordVerificationResult == PasswordVerificationResult.Success;
        }

        public async Task<UserTableModelDTO> CreateUserAsync(UserTableModelDTO userModel)
        {
            var role = await _context.UserRoleModelDTO.FirstOrDefaultAsync(r => r.Id == userModel.RoleId);
            if (role != null)
            {
                var newUser = new UserTableModelDTO
                {
                    Name = userModel.Name,
                    RoleId = role.Id,
                    Email = userModel.Email,
                    Password = HashPassword(userModel.Password)
                };
                _context.UserTableModel.Add(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            return null;
        }
        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<UserTableModelDTO>();
            return passwordHasher.HashPassword(null, password);
        }
    }
}
