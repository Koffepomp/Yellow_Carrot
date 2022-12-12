using Yellow_Carrot.Data;

namespace Yellow_Carrot.Managers
{
    public class RecipeManager
    {
        private readonly RecipeDbContext context;
        public RecipeManager(RecipeDbContext context)
        {
            this.context = context;
        }
    }
}
