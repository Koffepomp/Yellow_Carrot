using EntityFrameworkCore.EncryptColumn.Attribute;
using System.ComponentModel.DataAnnotations;

namespace Yellow_Carrot.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; } = null!;

        [EncryptColumn]
        public string Password { get; set; } = null!;

        public bool IsAdmin { get; set; }
    }
}
