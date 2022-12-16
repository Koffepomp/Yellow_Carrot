using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Yellow_Carrot.Data;
using Yellow_Carrot.Managers;
using Yellow_Carrot.Models;

namespace Yellow_Carrot
{
    /// <summary>
    /// Interaction logic for RecipeWindow.xaml
    /// </summary>
    public partial class RecipeWindow : Window
    {
        int loginId;
        private bool adminStatus = false;
        public RecipeWindow(int loginId, bool adminStatus)
        {
            InitializeComponent();
            // Tar emot UserId av användaren som loggat in och sparas så jag kan använda det i RecipeWindow.
            this.loginId = loginId;
            // Samma som UserId så sparas IsAdmin från användarens konto så man får rättigheter eller inte till att ta bort alla recept.
            this.adminStatus = adminStatus;
            LoadDbRecipes();
        }

        // Hämtar alla recept i databasen genom RecipeManager.
        // Om man är admin så visas en text som säger detta.
        public void LoadDbRecipes()
        {
            using (RecipeDbContext context = new())
            {
                UnitOfWork unitofwork = new(context);
                AddRecipesToListView(unitofwork.rManager.GetAllRecipes());
            }
            if (adminStatus)
            {
                lblAdminView.Visibility = Visibility.Visible;
            }
            else
            {
                lblAdminView.Visibility = Visibility.Hidden;
            }
        }

        // Används av LoadDbRecipes för att fylla listviewen med alla recept som hämtats.
        // Denna metod filtrerar också listviewen när sökfunktionen körs.
        private void AddRecipesToListView(List<Recipe> recipes)
        {
            using (UserDbContext context = new())
            {
                UserManager uManager = new(context);
                lvRecipes.Items.Clear();
                foreach (Recipe recipe in recipes)
                {
                    ListViewItem newItem = new();
                    newItem.Content = $"[{uManager.GetUserNameFromId(recipe.UserId)}] {recipe.Name}";
                    newItem.Tag = recipe;
                    lvRecipes.Items.Add(newItem);
                }
            }
        }

        // Öppnar AddRecipeWindow när man trycker på knappen och sätter RecipeWindow till ägare.
        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            AddRecipeWindow addrecipewindow = new(loginId);
            addrecipewindow.Owner = this;
            addrecipewindow.Show();
            this.Hide();
        }

        // Tar bort ett specifikt recept from listview och databasen så länge man äger receptet eller är admin.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem selectedItem = lvRecipes.SelectedItem as ListViewItem;
            Recipe selectedRecipe = selectedItem.Tag as Recipe;

            if (selectedRecipe.UserId == loginId || adminStatus)
            {
                // Frågar användaren om man är säker på att ta bort receptet.
                if (MessageBox.Show("Are you sure you want to delete this recipe?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Delete recipe
                    using (RecipeDbContext context = new())
                    {
                        UnitOfWork unitofwork = new(context);
                        unitofwork.rManager.DeleteRecipe(selectedRecipe);
                        context.SaveChanges();
                        LoadDbRecipes();
                    }
                    btnDelete.Visibility = Visibility.Hidden;
                    btnDetails.IsEnabled = false;
                }
                //else
                //{
                //
                //}
            }
            else
            {
                MessageBox.Show("You're not allowed to delete this recipe!");
            }
        }

        // Öppnar DetailsWindow och skickar med valt recept från listviewen samt sätter unlock rättigheter beroende på admin status.
        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            // hämta listview item tag och skicka med fönster
            ListViewItem selectedItem = lvRecipes.SelectedItem as ListViewItem;
            Recipe selectedRecipe = selectedItem.Tag as Recipe;
            bool unlockRights = selectedRecipe.UserId == loginId || adminStatus;

            DetailsWindow detailswindow = new(selectedRecipe.RecipeId, unlockRights);
            detailswindow.Owner = this;
            detailswindow.Show();
            this.Hide();
        }

        // Kör metoden SearchResult i RecipeManager. Skickar en sträng och får tillbaka en lista med recept som matchar sökordet.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            using (RecipeDbContext context = new())
            {
                UnitOfWork unitofwork = new(context);
                AddRecipesToListView(unitofwork.rManager.SearchResult(tbSearch.Text));
            }
        }

        // Stänger RecipeWindow och öppnar ägaren, vilket i detta fallet är MainWindow.
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        // När man väljer ett recept i listviewen så dyker "Delete" knappen upp samt "Details" knappen är inte grayed out längre.
        private void lvRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDelete.Visibility = Visibility.Visible;
            btnDetails.IsEnabled = true;
        }
    }
}
