using Yellow_Carrot.Data;

namespace Yellow_Carrot.Managers
{
    public class UserManager
    {
        private readonly UserDbContext context;

        public UserManager(UserDbContext context)
        {
            this.context = context;
        }
    }
}
