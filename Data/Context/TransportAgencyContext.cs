using Microsoft.EntityFrameworkCore;
using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Context
{
    public class TransportAgencyContext : DbContext
    {
        public TransportAgencyContext(DbContextOptions<TransportAgencyContext> options) : base(options)
        {
        }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<Models.Entities.Route> Routes { get; set; } // ← CAMBIO AQUÍ
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bus Configuration
            modelBuilder.Entity<Bus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Capacity).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.PlateNumber).IsUnique();
            });

            // Route Configuration - ← CAMBIO AQUÍ TAMBIÉN
            modelBuilder.Entity<Models.Entities.Route>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Origin).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Destination).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Distance).HasColumnType("decimal(6,2)");
                entity.Property(e => e.BasePrice).HasColumnType("decimal(8,2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Trip Configuration
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(8,2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(t => t.Bus)
                    .WithMany(b => b.Trips)
                    .HasForeignKey(t => t.BusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Route)
                    .WithMany(r => r.Trips)
                    .HasForeignKey(t => t.RouteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seat Configuration
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SeatNumber).IsRequired().HasMaxLength(5);
                entity.Property(e => e.IsOccupied).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(s => s.Trip)
                    .WithMany(t => t.Seats)
                    .HasForeignKey(s => s.TripId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.TripId, e.SeatNumber }).IsUnique();
            });

            // Customer Configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.DocumentNumber).IsUnique();
            });

            // Sale Configuration
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(8,2)");
                entity.Property(e => e.SaleDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ReceiptNumber).IsRequired().HasMaxLength(20);

                entity.HasOne(s => s.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Seat)
                    .WithMany(se => se.Sales)
                    .HasForeignKey(s => s.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ReceiptNumber).IsUnique();
            });
        }
    }
}