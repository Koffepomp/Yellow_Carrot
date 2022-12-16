using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Yellow_Carrot.Data;
using Yellow_Carrot.Models;

namespace Yellow_Carrot.Managers
{
    public class RecipeManager
    {
        private readonly RecipeDbContext context;
        public RecipeManager(RecipeDbContext context)
        {
            this.context = context;
        }

        // Hämtar alla recept i databasen samt inkluderar steps och tags.
        public List<Recipe> GetAllRecipes()
        {
            return context.Recipes.Include(r => r.Ingredients).Include(r => r.Steps).Include(r => r.Tags).ToList();
        }

        // Hämtar ett specifikt recept baserat på en int som skickas med. Inkluderar även här steps och tags.
        public Recipe? GetSpecificRecipe(int id)
        {
            return context.Recipes.Where(recipe => recipe.RecipeId == id).Include(recipe => recipe.Ingredients).Include(recipe => recipe.Steps).Include(recipe => recipe.Tags).FirstOrDefault();
        }

        // Används av AddRecipeWindow och skapar helt enkelt ett nytt recept.
        public void CreateNewRecipe(Recipe recipe)
        {
            context.Recipes.Add(recipe);
        }

        // Används av RecipeWindow när man väljer att ta bort ett recept.
        public void DeleteRecipe(Recipe recipeToRemove)
        {
            context.Recipes.Remove(recipeToRemove);
        }

        // Tar emot en sträng med sökord som matchas i databasen mot recept namn eller tags
        public List<Recipe> SearchResult(string keyword)
        {
            List<Recipe> result = context.Recipes.Where(r => r.Name.ToLower()
            .Contains(keyword) || r.Tags
            .Any(t => t.Name.ToLower() == keyword.ToLower()))
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Include(r => r.Tags).ToList();
            return result;
        }

        // Letar efter en tag i databasen med hjälp av en sträng
        public Tag? GetTagByName(string tagName)
        {
            return context.Tags.Where(t => t.Name == tagName).FirstOrDefault();
        }
    }
}
