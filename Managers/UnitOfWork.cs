using Yellow_Carrot.Data;

namespace Yellow_Carrot.Managers
{
    public class UnitOfWork
    {
        private readonly RecipeDbContext recipecontext;
        private readonly UserDbContext usercontext;
        private RecipeManager _rManager;
        private UserManager _uManager;

        public UnitOfWork(RecipeDbContext recipecontext, UserDbContext usercontext)
        {
            this.recipecontext = recipecontext;
            this.usercontext = usercontext;
        }

        public RecipeManager rManager
        {
            get
            {
                if (rManager == null)
                {
                    _rManager = new RecipeManager(recipecontext);
                }
                return _rManager;
            }
        }
        public UserManager uManager
        {
            get
            {
                if (uManager == null)
                {
                    _uManager = new UserManager(usercontext);
                }
                return _uManager;
            }
        }
    }
}
