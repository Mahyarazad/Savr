using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Savr.Persistence.Data
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) { }

        public DbSet<LogEntry> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("logs"); // Lowercase table name for PostgreSQL

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .IsRequired();

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .IsRequired();

                entity.Property(e => e.MessageTemplate)
                    .HasColumnName("message_template")
                    .IsRequired();

                entity.Property(e => e.Exception)
                    .HasColumnName("exception");

                entity.Property(e => e.Properties)
                    .HasColumnName("properties")
                    .HasColumnType("jsonb");

                entity.Property(e => e.LogEvent)
                    .HasColumnName("log_event")
                    .HasColumnType("jsonb");
            });
        }
    }

    public class LogEntry
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("timestamp", TypeName = "timestamp with time zone")]
        public DateTimeOffset Timestamp { get; set; }

        [Column("level")]
        public string Level { get; set; }

        [Column("message")]
        public string Message { get; set; }

        [Column("message_template")]
        public string MessageTemplate { get; set; }

        [Column("exception")]
        public string Exception { get; set; }

        [Column("properties", TypeName = "jsonb")]
        public string Properties { get; set; }

        [Column("log_event", TypeName = "jsonb")]
        public string LogEvent { get; set; }
    }
}
