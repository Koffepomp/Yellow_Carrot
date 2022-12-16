using System.Windows;
using Yellow_Carrot.Data;
using Yellow_Carrot.Managers;
using Yellow_Carrot.Models;

namespace Yellow_Carrot
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        // Förbereder en user att läggas in i databasen.
        // Newas upp med användarnamn, lösenord och en bool som avgör om det är ett admin konto eller inte.
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            User newUser = new()
            {
                Name = tbUsername.Text,
                Password = pbPassword.Password,
                IsAdmin = false
            };
            using (UserDbContext context = new())
            {
                UserManager user = new(context);

                // Lite checks så att man tar ett vettig användarnamn och lösenord.
                if (tbUsername.Text.Length >= 3 && tbUsername.Text.Length <= 16)
                {
                    if (pbPassword.Password.Length > 0)
                    {
                        if (user.CreateUser(newUser))
                        {
                            this.Owner.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Username is already taken!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a password.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a username 3-16 characters.");
                }
            }
        }

        // Stänger ner fönstret och öppnar ägaren, vilket i detta fallet är MainWindow.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}
