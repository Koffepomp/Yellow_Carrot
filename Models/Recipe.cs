using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yellow_Carrot.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public string Name { get; set; } = null!;
        public int UserId { get; set; }
        public List<Tag> Tags { get; set; } = new();
        public List<Step> Steps { get; set; } = new();
        public List<Ingredient> Ingredients { get; set; } = new();
    }
}
