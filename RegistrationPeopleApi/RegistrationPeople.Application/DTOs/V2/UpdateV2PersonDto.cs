namespace RegistrationPeople.Application.DTOs.V2
{
    public class UpdateV2PersonDto
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Birthplace { get; set; }
        public string? Nationality { get; set; }
        public string? Cpf { get; set; }

        public string? Address { get; set; }
    }
}
