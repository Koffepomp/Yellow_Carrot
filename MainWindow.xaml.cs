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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            using (UserDbContext context = new())
            {
                UserManager uManager = new(context);
                User? signedUser = uManager.LoginAuthentication(tbUsername.Text, pbPassword.Password);

                if (signedUser != null)
                {
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

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regWindow = new RegisterWindow();
            regWindow.Owner = this;
            regWindow.Show();
            this.Hide();
        }
    }
}
