using PruebaTecnicaAPI.Models.DTOs;
using PruebaTecnicaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PruebaTecnicaAPI.Services
{
    public class UserService
    {
        private readonly DbpruebaTecnicaContext _context;

        public UserService(DbpruebaTecnicaContext context)
        {
            _context = context;
        }

        public async Task<object> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _context.Users.Include(u => u.Phones).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return new { mensaje = "Usuario no encontrado" };

            user.Name = dto.Name;
            user.Modified = DateTime.UtcNow;

            _context.Phones.RemoveRange(user.Phones);
            foreach (var phone in dto.Phones)
            {
                _context.Phones.Add(new Phone
                {
                    UserId = user.Id,
                    Number = phone.Number,
                    CityCode = phone.CityCode,
                    CountryCode = phone.CountryCode
                });
            }

            await _context.SaveChangesAsync();

            return new { mensaje = "Usuario actualizado correctamente" };
        }

        public async Task<object> DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return new { mensaje = "Usuario no encontrado" };

            user.IsActive = false;
            user.Modified = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new { mensaje = "Usuario eliminado correctamente" };
        }

        public async Task<IEnumerable<object>> GetUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .Include(u => u.Phones)
                .ToListAsync();

            return users.Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                created = u.Created,
                modified = u.Modified,
                last_login = u.LastLogin,
                isactive = u.IsActive,
                phones = u.Phones.Select(p => new {
                    number = p.Number,
                    cityCode = p.CityCode,
                    countryCode = p.CountryCode
                })
            });
        }

        public async Task<object> GetCurrentUserAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Phones)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null)
                return new { mensaje = "Usuario no encontrado o inactivo" };

            return new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                created = user.Created,
                modified = user.Modified,
                last_login = user.LastLogin,
                isactive = user.IsActive,
                phones = user.Phones.Select(p => new {
                    number = p.Number,
                    cityCode = p.CityCode,
                    countryCode = p.CountryCode
                })
            };
        }


    }
}
