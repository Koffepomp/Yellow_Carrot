using System.Linq;
using Yellow_Carrot.Data;
using Yellow_Carrot.Models;

namespace Yellow_Carrot.Managers
{
    public class UserManager
    {
        private readonly UserDbContext context;

        public UserManager(UserDbContext context)
        {
            this.context = context;
        }

        public bool CreateUser(User newUser)
        {
            if (IsUsernameAvailable(newUser.Name))
            {
                context.Users.Add(newUser);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        private bool IsUsernameAvailable(string username)
        {
            User? u = context.Users.Where(u => u.Name == username).FirstOrDefault();
            if (u != null)
            {
                return false;
            }
            return true;
        }

        public User? LoginAuthentication(string userName, string password)
        {
            User? signedUser = context.Users.Where(u => u.Name == userName && u.Password == password).FirstOrDefault();

            return signedUser;
        }

        public string GetUserNameFromId(int id)
        {
            User? user = context.Users.Where(u => u.UserId == id).FirstOrDefault();
            return user.Name;
        }
    }
}
