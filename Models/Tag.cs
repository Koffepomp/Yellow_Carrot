using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yellow_Carrot.Models
{
    public class Tag
    {
        [Key]
        public string Name { get; set; } = null!;
        public List<Recipe> Recipes { get; set; } = new();
    }
}
