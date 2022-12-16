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

        public List<Recipe> GetAllRecipes()
        {
            return context.Recipes.Include(r => r.Ingredients).Include(r => r.Steps).Include(r => r.Tags).ToList();
        }

        public Recipe? GetSpecificRecipe(int id)
        {
            return context.Recipes.Where(recipe => recipe.RecipeId == id).Include(recipe => recipe.Ingredients).Include(recipe => recipe.Steps).Include(recipe => recipe.Tags).FirstOrDefault();
        }

        public void CreateNewRecipe(Recipe recipe)
        {
            context.Recipes.Add(recipe);
        }

        public void DeleteRecipe(Recipe recipeToRemove)
        {
            context.Recipes.Remove(recipeToRemove);
        }

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

        public Tag? GetTagByName(string tagName)
        {
            return context.Tags.Where(t => t.Name == tagName).FirstOrDefault();
        }
    }
}
