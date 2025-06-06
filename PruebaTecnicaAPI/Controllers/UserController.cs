using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaAPI.Models.DTOs;
using PruebaTecnicaAPI.Services;
using System.Security.Claims;

namespace PruebaTecnicaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { mensaje = "Token inválido o no presente" });

            var userId = Guid.Parse(userIdClaim.Value);
            var result = await _userService.UpdateUserAsync(userId, dto);

            if (result is IDictionary<string, object> dict && dict.ContainsKey("mensaje") && (string)dict["mensaje"] != "Usuario actualizado correctamente")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { mensaje = "Token inválido o no presente" });

            var userId = Guid.Parse(userIdClaim.Value);
            var result = await _userService.DeleteUserAsync(userId);

            if (result is IDictionary<string, object> dict && dict.ContainsKey("mensaje") && (string)dict["mensaje"] != "Usuario eliminado correctamente")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { mensaje = "Token inválido o no presente" });

            var result = await _userService.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { mensaje = "Token inválido o no presente" });

            var userId = Guid.Parse(userIdClaim.Value);
            var result = await _userService.GetCurrentUserAsync(userId);

            if (result is IDictionary<string, object> dict && dict.ContainsKey("mensaje"))
                return NotFound(result);

            return Ok(result);
        }


    }
}
