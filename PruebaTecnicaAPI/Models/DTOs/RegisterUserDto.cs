namespace PruebaTecnicaAPI.Models.DTOs
{
    public class RegisterUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<PhoneDto> Phones { get; set; }
    }
}
