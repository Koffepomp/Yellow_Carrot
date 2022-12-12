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
    }
}
