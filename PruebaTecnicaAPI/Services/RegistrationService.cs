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
            // Validar email con regex
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(dto.Email))
                return new { mensaje = "El formato del correo es inválido" };

            // Validar contraseña con regex configurable
            var passwordRegex = new Regex(_configuration["PasswordRegex"] ?? "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$");
            if (!passwordRegex.IsMatch(dto.Password))
                return new { mensaje = "La contraseña no cumple con los requisitos de seguridad" };

            // Verificar si el email ya existe
            if (_context.Users.Any(u => u.Email == dto.Email))
                return new { mensaje = "El correo ya registrado" };

            // Encriptar contraseña
            var encryptedPassword = EncriptSHA256(dto.Password);

            // Crear usuario
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Password = encryptedPassword,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "", // Temporal, se actualiza luego
                IsActive = true
            };

            // Agregar teléfonos
            foreach (var phone in dto.Phones)
            {
                user.Phones.Add(new Phone
                {
                    Id = 0, // será autogenerado
                    Number = phone.Number,
                    CityCode = phone.CityCode,
                    CountryCode = phone.CountryCode,
                    UserId = user.Id
                });
            }

            // Generar token y asignar
            var token = generateJWT(user);
            user.Token = token;

            // Guardar en DB
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Retornar datos esperados
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
