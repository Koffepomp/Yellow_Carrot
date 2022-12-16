using Microsoft.EntityFrameworkCore;
using Yellow_Carrot.Models;

namespace Yellow_Carrot.Data
{
    public class RecipeDbContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        public RecipeDbContext()
        {

        }

        public RecipeDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            optionsbuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=YellowCarrotRecipeDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>().HasMany(u => u.Ingredients).WithOne(r => r.Recipe).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Recipe>().HasMany(u => u.Steps).WithOne(r => r.Recipe).OnDelete(DeleteBehavior.Cascade);

            // Recipe #1 seed
            modelBuilder.Entity<Recipe>().HasData
                (new Recipe()
                {
                    RecipeId = 1,
                    Name = "Köttfärssås2000",
                    UserId = 1,
                });

            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient()
                {
                    IngredientId = 1,
                    Name = "Köttfärs",
                    Quantity = "",
                    RecipeId = 1,
                },
                new Ingredient()
                {
                    IngredientId = 2,
                    Name = "Sås",
                    Quantity = "",
                    RecipeId = 1,
                },
                new Ingredient()
                {
                    IngredientId = 3,
                    Name = "Champis",
                    Quantity = "1",
                    RecipeId = 1,
                },
                new Ingredient()
                {
                    IngredientId = 4,
                    Name = "Senap",
                    Quantity = "1",
                    RecipeId = 1,
                });

            modelBuilder.Entity<Step>().HasData(
                new Step()
                {
                    StepId = 1,
                    Order = 1,
                    Description = "Stek köttfärsen",
                    RecipeId = 1,
                },
                new Step()
                {
                    StepId = 2,
                    Order = 2,
                    Description = "Blanda champis och senap i ett glas",
                    RecipeId = 1,
                },
                new Step()
                {
                    StepId = 3,
                    Order = 3,
                    Description = "Ta en klunk för att komma i stämning",
                    RecipeId = 1,
                },
                new Step()
                {
                    StepId = 4,
                    Order = 4,
                    Description = "Häll såsen i köttfärsen",
                    RecipeId = 1,
                },
                new Step()
                {
                    StepId = 5,
                    Order = 5,
                    Description = "Servera och njut",
                    RecipeId = 1,
                });
        }
    }
}
