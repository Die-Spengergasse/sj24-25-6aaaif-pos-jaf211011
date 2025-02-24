using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe1.Infrastructure
{
    public class AppointmentContext : DbContext
    {
        // TODO: Add your DbSets here
        public DbSet<PaymentItem> PaymentItems => Set<PaymentItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<CashDesk> CashDesks => Set<CashDesk>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Cashier> Cashiers => Set<Cashier>();
        public AppointmentContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasDiscriminator(e => e.Type);
        }
    }
}