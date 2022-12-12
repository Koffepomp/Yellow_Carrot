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
            this._provider = new GenerateEncryptionProvider("_example_encryption_key_");
        }

        public UserDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            optionsbuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=YellowCarrotUserDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(_provider);
        }
    }
}
