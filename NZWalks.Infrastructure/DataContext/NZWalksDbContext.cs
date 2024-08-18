using Microsoft.EntityFrameworkCore;
using NZWalks.Core.Domain.Entities;

namespace NZWalks.Infrastructure.DataContext
{
    public class NZWalksDbContext : DbContext // Represent a connection with database and hold a set of DBSets
    {
        // send our own connection through the program.cs
        // phải thêm Generic ...<NZWalksDbContext> để khỏi bị lỗi nếu trong project có thêm DbContext
        // mới khác (ví dụ: NZWalksAuthDbContext)
        public NZWalksDbContext(DbContextOptions<NZWalksDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        // DBSet is a property of DBContext that represents a collection of entities in the database (Table)
        // These property will create table inside database
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }

        // Seeding Data Using EF Core
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set name table of entity (phải là số nhiều, ko set thì nó sẽ lấy default)
            //modelBuilder.Entity<Difficulty>().ToTable("Difficulties");


            // Seed data for Difficulties (Easy, Medium, Hard)
            var difficulties = new List<Difficulty>()
            {
                new() {
                    Id = Guid.Parse("269c567a-bd7d-47d3-902d-85d52d583aa9"),
                    Name = "Easy"
                },
                new Difficulty
                {
                    Id = Guid.Parse("4c90f142-af3e-49cf-a9d1-677883d94cbb"),
                    Name = "Medium"
                },
                new Difficulty
                {
                    Id = Guid.Parse("242b380d-b470-4026-bf32-9fdbdbce0a04"),
                    Name = "Hard"
                },
            };

            modelBuilder.Entity<Difficulty>().HasData(difficulties);

            // Seed data for regions
            var regions = new List<Region>()
            {
                new Region
                {
                    Id = Guid.Parse("fdea4f6b-17c8-46cf-8806-ec97ba4f595a"),
                    Code = "USA",
                    Name = "United States America",
                    RegionImageUrl = "https://image-usa.com"
                },
                new Region
                {
                    Id = Guid.Parse("050f77b8-5c50-4d1e-9aaf-e034c018cbea"),
                    Code = "UK",
                    Name = "United Kingdom",
                    RegionImageUrl = "https://image-uk.com"
                },
                new Region
                {
                    Id = Guid.Parse("05fa9baa-e2de-422d-9420-87531c6c468b"),
                    Code = "CN",
                    Name = "China",
                    RegionImageUrl = "https://image-cn.com"
                },
                new Region
                {
                    Id = Guid.Parse("59201bbd-ff5d-49ba-9aa7-6d4427a2069b"),
                    Code = "Russia",
                    Name = "RS",
                    RegionImageUrl = "https://image-rs.com"
                },
                new Region
                {
                    Id = Guid.Parse("9c0b21ba-66bf-4a0c-8126-3a59415bdac3"),
                    Code = "Wellington",
                    Name = "WGN",
                    RegionImageUrl = null
                },
                new Region
                {
                    Id = Guid.Parse("0cc09215-9a9a-4fc9-8408-e27b68b3b4dd"),
                    Code = "SouthLand",
                    Name = "STL",
                    RegionImageUrl = null
                },
            };

            modelBuilder.Entity<Region>().HasData(regions);

            // Seed data for Walk

        }
    }
}
