using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using PruebaTecnicaAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using PruebaTecnicaAPI.Models.DTOs;
using System.Text.RegularExpressions;

namespace PruebaTecnicaAPI.Services
{
    public class RegistrationService
    {
        private readonly IConfiguration _configuration;
        private readonly DbpruebaTecnicaContext _context;

        public RegistrationService(IConfiguration configuration, DbpruebaTecnicaContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string EncriptSHA256(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[]bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public string generateJWT(User user)
        {
            var userClaim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var JwtConfig = new JwtSecurityToken(
                claims: userClaim,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken( JwtConfig );
        }

        public async Task<object> RegistrarUsuarioAsync(RegisterUserDto dto)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(dto.Email))
                return new { mensaje = "El formato del correo es inválido" };

            var passwordRegex = new Regex(_configuration["PasswordRegex"] ?? "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$");
            if (!passwordRegex.IsMatch(dto.Password))
                return new { mensaje = "La contraseña no cumple con los requisitos de seguridad" };

            if (_context.Users.Any(u => u.Email == dto.Email))
                return new { mensaje = "El correo ya registrado" };

            var encryptedPassword = EncriptSHA256(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Password = encryptedPassword,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "", 
                IsActive = true
            };

            foreach (var phone in dto.Phones)
            {
                user.Phones.Add(new Phone
                {
                    Id = 0, 
                    Number = phone.Number,
                    CityCode = phone.CityCode,
                    CountryCode = phone.CountryCode,
                    UserId = user.Id
                });
            }

            var token = generateJWT(user);
            user.Token = token;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new
            {
                id = user.Id,
                created = user.Created,
                modified = user.Modified,
                last_login = user.LastLogin,
                token = user.Token,
                isactive = user.IsActive
            };
        }
    }
}
