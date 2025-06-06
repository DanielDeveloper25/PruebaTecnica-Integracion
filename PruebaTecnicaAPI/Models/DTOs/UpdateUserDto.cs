namespace PruebaTecnicaAPI.Models.DTOs
{
    public class UpdateUserDto
    {
        public string Name { get; set; }
        public List<PhoneDto> Phones { get; set; }
    }
}
