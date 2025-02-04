using Microsoft.EntityFrameworkCore;
using FlightApp.Models;

namespace FlightApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet ile mevcut tablolar
        public DbSet<User>? Users { get; set; } // Nullable olarak tanımlandı
        public required DbSet<UserProfile> UserProfiles { get; set; } // Görünüm için DbSet tanımı

        // Yeni tablolar
        public DbSet<Airline> Airlines { get; set; } = null!;
        public DbSet<Flight> Flights { get; set; } = null!;
        public DbSet<VwFlight> VwFlights { get; set; } = null!; // Default değer atanarak düzeltildi
        public DbSet<Passenger> Passengers { get; set; } = null!; // Passenger tablosu eklendi
        public DbSet<FlightClass> FlightClasses { get; set; } = null!; // FlightClass tablosu eklendi
        
        public DbSet<Reservation> Reservations { get; set; } = null!; // Reservation tablosu eklendi

        public DbSet<ReservationDetail> ReservationDetails { get; set; }= null!;

       public DbSet<AdminUserView> AdminUserView { get; set; }= null!;
        public DbSet<AdminPassengerView> AdminPassengerView { get; set; }= null!;

        // FlightDetails View tanımı
        public DbSet<FlightDetail> FlightDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservationDetail>()
            .HasNoKey()
            .ToView("ReservationDetails");

              modelBuilder.Entity<AdminUserView>()
            .HasNoKey()
            .ToView("AdminUserView");

                modelBuilder.Entity<AdminPassengerView>()
            .HasNoKey()
            .ToView("AdminPassengerView");

            // Görünüm tanımı
            modelBuilder.Entity<UserProfile>().HasNoKey().ToView("UserProfile");

            // FlightDetails View tanımı
            modelBuilder.Entity<FlightDetail>().HasNoKey().ToView("vw_FlightDetails");

            // VwFlights Görünümü Tanımı
            modelBuilder.Entity<VwFlight>().HasNoKey().ToView("vw_flights"); // 'vw_flights' view'ı için tanımlama

            // Fluent API ile yeni tabloların yapılandırması
            modelBuilder.Entity<Airline>(entity =>
            {
                entity.ToTable("airlines");
                entity.HasKey(a => a.AirlineId);
                entity.Property(a => a.AirlineName).IsRequired().HasMaxLength(100);
                entity.Property(a => a.AirlineCountry).HasMaxLength(50);
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.ToTable("flights");
                entity.HasKey(f => f.FlightId);
                entity.Property(f => f.DepartureCity).IsRequired().HasMaxLength(100);
                entity.Property(f => f.ArrivalCity).IsRequired().HasMaxLength(100);
                entity.Property(f => f.Price).HasColumnType("decimal(10,2)").IsRequired();

                // Foreign key ilişkisi
                entity.HasOne(f => f.Airline)
                      .WithMany()
                      .HasForeignKey(f => f.AirlineId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.ToTable("passengers");
                entity.HasKey(p => p.PassengerId);
                entity.Property(p => p.Gender).IsRequired().HasConversion<string>();
                entity.Property(p => p.TcNo).IsRequired().HasMaxLength(11);
                entity.Property(p => p.PassengerName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.PassengerSurname).IsRequired().HasMaxLength(100);
                entity.Property(p => p.DateOfBirth).IsRequired();
            });

            modelBuilder.Entity<FlightClass>(entity =>
            {
                entity.ToTable("flightclasses");
                entity.HasKey(fc => fc.ClassId);
                entity.Property(fc => fc.ClassName).IsRequired().HasMaxLength(100);
                entity.Property(fc => fc.PriceMultiplier).IsRequired().HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("reservations");
                entity.HasKey(r => r.ReservationId);
                entity.Property(r => r.ReservationDate).IsRequired();

                // Foreign key ilişkileri
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Flight>()
                      .WithMany()
                      .HasForeignKey(r => r.FlightId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Passenger>()
                      .WithMany()
                      .HasForeignKey(r => r.PassengerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<FlightClass>()
                      .WithMany()
                      .HasForeignKey(r => r.ClassId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
