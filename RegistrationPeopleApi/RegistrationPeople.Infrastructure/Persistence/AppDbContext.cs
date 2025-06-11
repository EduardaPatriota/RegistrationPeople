using Microsoft.EntityFrameworkCore;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Infrastructure.Persistence.Configurations;
using System.Collections.Generic;

namespace RegistrationPeople.Infrastructure.Persistence
{
    public class AppDbContext : DbContext

    {
        public DbSet<Person> People { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
