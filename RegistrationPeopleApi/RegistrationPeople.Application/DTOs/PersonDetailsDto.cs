using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationPeople.Application.DTOs
{
    public class PersonDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Cpf { get; set; }
        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Birthplace { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


}
