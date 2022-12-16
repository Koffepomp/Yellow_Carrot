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
            this.loginId = loginId;
            this.adminStatus = adminStatus;
            LoadDbRecipes();
        }

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

        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            AddRecipeWindow addrecipewindow = new(loginId);
            addrecipewindow.Owner = this;
            addrecipewindow.Show();
            this.Hide();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem selectedItem = lvRecipes.SelectedItem as ListViewItem;
            Recipe selectedRecipe = selectedItem.Tag as Recipe;

            if (selectedRecipe.UserId == loginId || adminStatus)
            {
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
                else
                {
                    // Nothing
                }
            }
            else
            {
                MessageBox.Show("You're not allowed to delete this recipe!");
            }
        }

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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            using (RecipeDbContext context = new())
            {
                UnitOfWork unitofwork = new(context);
                AddRecipesToListView(unitofwork.rManager.SearchResult(tbSearch.Text));
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void lvRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDelete.Visibility = Visibility.Visible;
            btnDetails.IsEnabled = true;
        }
    }
}
