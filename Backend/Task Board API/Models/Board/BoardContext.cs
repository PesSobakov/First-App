using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;


namespace Task_Board_API.Models.Board
{
    public class BoardContext : DbContext
    {
        public BoardContext()
        {
        }

        public BoardContext(DbContextOptions<BoardContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<BoardList> BoardLists { get; set; }
        public virtual DbSet<CardHistory> CardHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // string user = Environment.GetEnvironmentVariable("POSTGRES_USER");
           // string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
           // string db = Environment.GetEnvironmentVariable("POSTGRES_DB");
           // string server = Environment.GetEnvironmentVariable("POSTGRES_SERVER");
           // string port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
           //
           // optionsBuilder.UseNpgsql($"Server={server};Port={port};Database={db};User Id={user};Password={password};");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CardHistory>(entity =>
            {
                entity.HasOne(d => d.Card).WithMany(p => p.CardHistories)
                    .HasForeignKey(d => d.CardId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
    }
