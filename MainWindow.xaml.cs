using System.Windows;
using Yellow_Carrot.Data;
using Yellow_Carrot.Managers;
using Yellow_Carrot.Models;

namespace Yellow_Carrot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Kör en metod i UserManager som kollar att användarnamn och lösenord matchar ett konto i databasen.
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            using (UserDbContext context = new())
            {
                UserManager uManager = new(context);
                User? signedUser = uManager.LoginAuthentication(tbUsername.Text, pbPassword.Password);

                // Om ett konto hittas så returneras det hit, annars är det null och ett felmeddelande visas.
                if (signedUser != null)
                {
                    // Här skickas även med en bool som säger om användaren som loggar in är admin eller inte.
                    RecipeWindow recipeWindow = new(signedUser.UserId, signedUser.IsAdmin);
                    recipeWindow.Owner = this;
                    recipeWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Wrong username or password!");
                }
            }
        }

        // Sätter MainWindow till ägare och öppnar RegisterWindow.
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regWindow = new RegisterWindow();
            regWindow.Owner = this;
            regWindow.Show();
            this.Hide();
        }
    }
}
