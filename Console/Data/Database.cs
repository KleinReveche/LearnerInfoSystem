using Microsoft.EntityFrameworkCore;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Console.Data;

public abstract class DatabaseContext : DbContext
{
    public DbSet<Setting> Settings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Program> Programs { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<ProgramTracker> ProgramTrackers { get; set; }
    public DbSet<CourseCompletion> CourseCompletions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Setting>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<Program>().HasKey(x => x.Id);
        modelBuilder.Entity<Course>().HasKey(x => x.Id);
        modelBuilder.Entity<ProgramTracker>().HasKey(x => x.Id);
        modelBuilder.Entity<CourseCompletion>().HasKey(x => x.Id);
    }
}

public class SqliteDbContext : DatabaseContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseSqlite("Data Source=SimpleLearnerInfoSystem.db");
    }
}

public class MySqlDbContext : DatabaseContext
{
    private static readonly string MySqlConnectionString = $"server={Application.SqlCredentials.Host},{Application.SqlCredentials.Port};" +
                                                            $"database={Application.SqlCredentials.Database};" +
                                                            $"user={Application.SqlCredentials.Username};" +
                                                            $"password={Application.SqlCredentials.Password};";
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
// #if DEBUG
//         _mySqlConnectionString = "Server=localhost;Database=SimpleSchoolInfoSystem;User ID=test;Password=password;";
// #endif
        optionsBuilder.UseMySql(MySqlConnectionString, ServerVersion.AutoDetect(MySqlConnectionString));
    }
}