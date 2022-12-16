using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yellow_Carrot.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }
        public string Name { get; set; } = null!;
        public string Quantity { get; set; } = "";
        public Recipe Recipe { get; set; }
        [ForeignKey(nameof(Recipe))]
        public int RecipeId { get; set; }
    }
}
