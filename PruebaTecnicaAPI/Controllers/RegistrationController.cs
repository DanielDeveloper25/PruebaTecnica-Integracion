using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaAPI.Models.DTOs;
using PruebaTecnicaAPI.Services;

namespace PruebaTecnicaAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationService _registrationService;

        public RegistrationController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var result = await _registrationService.RegistrarUsuarioAsync(dto);

            if (result is IDictionary<string, object> dict && dict.ContainsKey("mensaje"))
            {
                return BadRequest(result); 
            }

            return Ok(result); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var result = await _registrationService.LoginUsuarioAsync(dto);

            if (result is IDictionary<string, object> dict && dict.ContainsKey("mensaje"))
                return BadRequest(result);

            return Ok(result);
        }
    }
}