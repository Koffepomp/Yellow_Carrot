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

        // Används av RegisterWindow för att skapa ett nytt konto.
        // Den använder metoden nedan också för att kolla om användarnamnet är tillgängligt.
        // Om inte så skapas en ny användare och returnerar en bool. 
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

        // Används i metoden ovan för att jämnföra en sträng mot databasen och kolla om ett nytt användarnamn är tillgängligt.
        // Returnerar som false om namnet är upptaget vilket får CreateUser att avbryta sin account creation.
        private bool IsUsernameAvailable(string username)
        {
            User? u = context.Users.Where(u => u.Name == username).FirstOrDefault();
            if (u != null)
            {
                return false;
            }
            return true;
        }

        // När man trycker på logga in så kollas användarnamn och lösenord mot databasen.
        // Om det inte matchar returneras en null user vilket avbryter inloggningen.
        public User? LoginAuthentication(string userName, string password)
        {
            User? signedUser = context.Users.Where(u => u.Name == userName && u.Password == password).FirstOrDefault();

            return signedUser;
        }

        // Returnerar ett användarnamn från databasen baserat på UserId som skickas av en int.
        public string GetUserNameFromId(int id)
        {
            User? user = context.Users.Where(u => u.UserId == id).FirstOrDefault();
            return user.Name;
        }
    }
}
