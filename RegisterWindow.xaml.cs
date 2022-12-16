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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}
