using RegistrationPeople.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

using System.Text.RegularExpressions;

public class RegisterV2PersonDto : IValidatableObject
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    public string Name { get; set; } = string.Empty;

    public string? Gender { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
    [DataType(DataType.Date, ErrorMessage = "Data de nascimento inválida.")]
    public DateTime BirthDate { get; set; }

    public string? Birthplace { get; set; }

    public string? Nationality { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório.")]
    public string Cpf { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Endereço é obrigatório.")]
    public string Address { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var cpfRegex = new Regex(@"^\d{11}$");
        if (!cpfRegex.IsMatch(Cpf))
        {
            yield return new ValidationResult("CPF inválido. Deve conter exatamente 11 números (sem pontos ou traços).", new[] { nameof(Cpf) });
        }

        if (BirthDate > DateTime.Today)
        {
            yield return new ValidationResult("Data de nascimento não pode ser futura.", new[] { nameof(BirthDate) });
        }

        var personRepository = validationContext.GetService(typeof(IPersonRepository)) as IPersonRepository;
        if (personRepository != null)
        {
            var exists = personRepository.ExistsCpfAsync(Cpf).GetAwaiter().GetResult();
            if (exists)
            {
                yield return new ValidationResult("CPF já cadastrado.", new[] { nameof(Cpf) });
            }
        }
        else
        {
            yield return new ValidationResult("Não foi possível validar a unicidade do CPF no momento.", new[] { nameof(Cpf) });
        }
    }
}
