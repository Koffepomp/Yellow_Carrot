using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yellow_Carrot.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public List<Recipe> Recipes { get; set; } = new();
    }
}
