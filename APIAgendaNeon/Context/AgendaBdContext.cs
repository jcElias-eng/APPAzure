using APIAgendaNeon.Models;
using Microsoft.EntityFrameworkCore;

namespace APIAgendaNeon.Context
{
    public class AgendaBDContext : DbContext
    {
        public AgendaBDContext(DbContextOptions<AgendaBDContext> options)
            : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persona>().ToTable("persona"); 
            base.OnModelCreating(modelBuilder);
        }
    }
}
