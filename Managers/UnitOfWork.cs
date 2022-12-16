using Yellow_Carrot.Data;

namespace Yellow_Carrot.Managers
{
    public class UnitOfWork
    {
        private readonly RecipeDbContext recipecontext;
        private RecipeManager _rManager;

        // Min UnitOfWork som hjälper till med databasen och metoder.
        public UnitOfWork(RecipeDbContext recipecontext)
        {
            this.recipecontext = recipecontext;
        }

        // RecipeManager där jag kör alla metoder som hanterar recepten.
        public RecipeManager rManager
        {
            get
            {
                if (_rManager == null)
                {
                    _rManager = new RecipeManager(recipecontext);
                }
                return _rManager;
            }
        }
    }
}
