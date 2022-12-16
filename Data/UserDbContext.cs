using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.EntityFrameworkCore;
using Yellow_Carrot.Models;

namespace Yellow_Carrot.Data
{
    public class UserDbContext : DbContext
    {
        private IEncryptionProvider _provider;
        public UserDbContext()
        {
            // Krypteringsnyckeln för att dölja password i databasen
            this._provider = new GenerateEncryptionProvider("_example_encryption_key_");
        }

        public UserDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            // Sätter min andra databas till YellowCarrotUserDb som endast hanterar users
            optionsbuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=YellowCarrotUserDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(_provider);

            // Skapar admin och user konton, som får rättigheterna till att vara admin eller inte
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    UserId = 1,
                    Name = "admin",
                    Password = "password",
                    IsAdmin = true,
                },
                new User()
                {
                    UserId = 2,
                    Name = "user",
                    Password = "password",
                    IsAdmin = false,
                });
        }
    }
}
