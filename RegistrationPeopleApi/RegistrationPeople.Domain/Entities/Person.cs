using Microsoft.AspNetCore.Identity;

namespace RegistrationPeople.Domain.Entities
{
    public class Person : IdentityUser<Guid>
    {      
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Birthplace { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public Person()
        {
            Id = Guid.NewGuid(); 
        }

       


    }
}