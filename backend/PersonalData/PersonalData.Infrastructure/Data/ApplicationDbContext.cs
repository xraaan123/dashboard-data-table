using Microsoft.EntityFrameworkCore;
using PersonalData.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PersonalData.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<PersonEntity> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PersonEntity
            modelBuilder.Entity<PersonEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasAnnotation("SqlServer:Identity", "1, 1");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnType("nvarchar(500)");

                entity.Property(e => e.BirthDate)
                    .IsRequired()
                    .HasColumnType("date")
                    .HasColumnType("date");

                entity.Property(e => e.Age)
                    .IsRequired()
                    .HasColumnType("int");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false)
                    .HasColumnType("datetime2");

                // Indexes for better performance
                entity.HasIndex(e => new { e.FirstName, e.LastName })
                    .HasDatabaseName("IX_Person_FullName");
            });

            // Seed data for testing
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedPersons = new[]
            {
                new
                {
                    Id = 1,
                    FirstName = "สมชาย",
                    LastName = "ใจดี",
                    Address = "123/45 หมู่ 2 ตำบลบางกะปิ เขตห้วยขวาง กรุงเทพมหานคร 10310",
                    BirthDate = new DateTime(1990, 5, 15),
                    Age = DateTime.Today.Year - 1990 - (DateTime.Today < new DateTime(DateTime.Today.Year, 5, 15) ? 1 : 0),
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 2,
                    FirstName = "สมศรี",
                    LastName = "รักดี",
                    Address = "456/78 ซอยลาดพร้าว 15 แขวงจตุจักร เขตจตุจักร กรุงเทพมหานคร 10900",
                    BirthDate = new DateTime(1985, 8, 22),
                    Age = DateTime.Today.Year - 1985 - (DateTime.Today < new DateTime(DateTime.Today.Year, 8, 22) ? 1 : 0),
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 3,
                    FirstName = "วิชัย",
                    LastName = "สุขใส",
                    Address = "789/12 ถนนสุขุมวิท แขวงคลองเตย เขตคลองเตย กรุงเทพมหานคร 10110",
                    BirthDate = new DateTime(1992, 12, 3),
                    Age = DateTime.Today.Year - 1992 - (DateTime.Today < new DateTime(DateTime.Today.Year, 12, 3) ? 1 : 0),
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 4,
                    FirstName = "ปิยะดา",
                    LastName = "หวานใจ",
                    Address = "321/67 หมู่บ้านสีลม ตำบลสีลม เขตบางรัก กรุงเทพมหานคร 10500",
                    BirthDate = new DateTime(1988, 3, 18),
                    Age = DateTime.Today.Year - 1988 - (DateTime.Today < new DateTime(DateTime.Today.Year, 3, 18) ? 1 : 0),
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 5,
                    FirstName = "อนันต์",
                    LastName = "มีสุข",
                    Address = "555/88 ซอยอารีย์ แขวงสามเสนใน เขตพญาไท กรุงเทพมหานคร 10400",
                    BirthDate = new DateTime(1995, 7, 9),
                    Age = DateTime.Today.Year - 1995 - (DateTime.Today < new DateTime(DateTime.Today.Year, 7, 9) ? 1 : 0),
                    CreatedAt = DateTime.UtcNow
                }
            };

            modelBuilder.Entity<PersonEntity>().HasData(seedPersons);
        }
    }
}
