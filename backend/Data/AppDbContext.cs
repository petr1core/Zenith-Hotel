using Hotel_MVP.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hotel_MVP.Data
{
    public class AppDbContext : DbContext
    {
        // Dependency Injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // EF Core Tools
        public AppDbContext()
        {
        }

        // DbSet для каждой таблицы
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<GuestProfile> GuestProfiles { get; set; }
        //public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomPhoto> RoomPhotos { get; set; }
        public DbSet<RoomRating> RoomRatings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<ServiceEmployee> ServiceEmployees { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Настройте подключение к базе данных напрямую (используется только для EF Core Tools)
                optionsBuilder.UseNpgsql("Host=localhost;Database=your_database_name;Username=postgres;Password=your_password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<AdminProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<GuestProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
                entity.HasMany(r => r.Images)
                      .WithOne(i => i.Room)
                      .HasForeignKey(i => i.RoomId);
            });

            modelBuilder.Entity<RoomPhoto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.RoomId).HasColumnName("roomId");
            });


            modelBuilder.Entity<RoomRating>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<ServiceOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            modelBuilder.Entity<EmployeeSalary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            });

            // modelBuilder.Entity<Payment>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //     entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
            // });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);
            });

        }
    }
}