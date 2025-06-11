using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationPeople.Application.DTOs
{
    
    public class PersonSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Cpf { get; set; }
        public string? Address { get; set; }
    }
}
