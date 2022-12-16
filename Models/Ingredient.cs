using System.ComponentModel.DataAnnotations;

namespace Yellow_Carrot.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }
        public string Name { get; set; } = null!;
        public string Quantity { get; set; } = "";
        public Recipe Recipe { get; set; } = new();
        public int RecipeId { get; set; }
    }
}
