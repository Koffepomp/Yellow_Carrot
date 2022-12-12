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
        public RecipeWindow(int loginId)
        {
            InitializeComponent();
            LoadRecipes();
        }

        private void LoadRecipes()
        {
            using (RecipeDbContext context = new())
            {
                UnitOfWork uow = new(context);
                List<Recipe> recipes = uow.rManager.GetAllRecipes();
                AddRecipesToListView(recipes);

            }
        }

        private void AddRecipesToListView(List<Recipe> recipes)
        {
            foreach (Recipe recipe in recipes)
            {
                ListViewItem newItem = new();
                newItem.Content = recipe.Name;
                newItem.Tag = recipe;
            }
        }

        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
