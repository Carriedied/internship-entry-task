using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);

                var converter = new ValueConverter<string[][], string>(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<string[][]>(v, (JsonSerializerOptions?)null)
                );

                var comparer = new ValueComparer<string[][]>(
                    (c1, c2) => c1 == c2 || (c1 != null && c2 != null && JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null)),
                    c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                    c => c == null ? null : JsonSerializer.Deserialize<string[][]>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)
                );

                entity.Property(e => e.Board)
                      .HasConversion(converter, comparer)
                      .HasColumnType("jsonb");
            });
        }
    }
}
