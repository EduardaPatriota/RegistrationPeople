using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegistrationPeople.Domain.Entities;


namespace RegistrationPeople.Infrastructure.Persistence.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("People");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.Cpf)
                   .IsUnique();

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Cpf)
                   .IsRequired()
                   .HasMaxLength(11);

            builder.Property(p => p.BirthDate)
                   .IsRequired();

            builder.Property(p => p.Gender)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired(false);

            builder.Property(p => p.Birthplace)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(p => p.Nationality)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(p => p.Address)
                   .HasMaxLength(300)
                   .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.UpdatedAt)
                   .IsRequired();
        }
    }
}