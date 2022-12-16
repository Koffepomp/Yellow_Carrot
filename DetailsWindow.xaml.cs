using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Yellow_Carrot.Data;
using Yellow_Carrot.Managers;
using Yellow_Carrot.Models;

namespace Yellow_Carrot
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        private bool unlockRights = false;
        private int currentRecipeId;
        public DetailsWindow(int currentRecipeId, bool unlockRights)
        {
            InitializeComponent();
            this.currentRecipeId = currentRecipeId;
            this.unlockRights = unlockRights;

            LoadRecipeData();
        }

        // Denna metoden körs bara 1 gång när DetailsWindow öppnas, vilket fyller i all information gällande receptet som man valt.
        private void LoadRecipeData()
        {
            using (RecipeDbContext context = new())
            {
                UnitOfWork unitofwork = new(context);
                Recipe currentRecipe = unitofwork.rManager.GetSpecificRecipe(currentRecipeId);
                tbRecipeName.Text = currentRecipe.Name;

                // Lägger till alla nuvarande ingredienser i listviewen.
                foreach (Ingredient ingredient in currentRecipe.Ingredients)
                {
                    ListViewItem oldItem = new();
                    oldItem.Content = ingredient.Name;
                    if (ingredient.Quantity != "")
                    {
                        oldItem.Content += $" x{ingredient.Quantity}";
                    }
                    oldItem.Tag = ingredient;
                    lvIngredients.Items.Add(oldItem);
                }

                // Lägger till alla nuvarande steps i listviewen.
                foreach (Step step in currentRecipe.Steps)
                {
                    ListViewItem oldItem = new();
                    oldItem.Content = $"{step.Order}) {step.Description}";
                    oldItem.Tag = step;
                    lvSteps.Items.Add(oldItem);
                }

                // Lägger till alla nuvarande tags i listviewen.
                foreach (Tag tag in currentRecipe.Tags)
                {
                    ListViewItem oldItem = new();
                    oldItem.Content = $"#{tag.Name}";
                    oldItem.Tag = tag;
                    lvTags.Items.Add(oldItem);
                }
            }
        }

        // När man trycker på unlock så kollar den om unlockRights är satt till true, baserat på IsAdmin propertyn på kontot.
        // Om den är true så låses alla textrutor och listviews upp och man kan lägga till fler ingredienser, steps eller tags.
        private void btnUnlock_Click(object sender, RoutedEventArgs e)
        {
            if (unlockRights)
            {
                // Ingredients elements
                lvIngredients.IsEnabled = true;
                lblIngredientName.Visibility = Visibility.Visible;
                tbAddIngredient.Visibility = Visibility.Visible;
                lblQuantity.Visibility = Visibility.Visible;
                tbAddQuantity.Visibility = Visibility.Visible;
                btnAddIngredient.Visibility = Visibility.Visible;

                // Steps elements
                lvSteps.IsEnabled = true;
                tbAddStep.Visibility = Visibility.Visible;
                btnAddStep.Visibility = Visibility.Visible;

                // Tags elements
                lvTags.IsEnabled = true;
                tbAddTag.Visibility = Visibility.Visible;
                btnAddTag.Visibility = Visibility.Visible;

                // Låser upp recept namn, stänger av unlock knappen och save knappen är inte grayed out längre.
                tbRecipeName.IsEnabled = true;
                btnUnlock.IsEnabled = false;
                btnSave.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("You can't edit this recipe!");
            }

        }

        // Stänger fönstret och öppnar ägaren, vilket i detta fallet är RecipeWindow.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        // Sparar alla listviews, både ingredienser, steps och tags. Därefter ersätter den gamla informationen med den nya.
        // Det sista metoden gör är att casta en metod ifrån RecipeWindow vid namn "LoadsDbRecipes" vilket uppdaterar listviewen.
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (RecipeDbContext context = new())
            {
                UnitOfWork unitofwork = new(context);
                Recipe oldRecipe = unitofwork.rManager.GetSpecificRecipe(currentRecipeId);

                List<Ingredient> newIngredients = new();
                foreach (ListViewItem item in lvIngredients.Items)
                {
                    Ingredient ingredient = item.Tag as Ingredient;
                    newIngredients.Add(ingredient);
                }

                int stepOrder = 1;
                List<Step> newSteps = new();
                foreach (ListViewItem item in lvSteps.Items)
                {
                    Step step = item.Tag as Step;
                    step.Order = stepOrder;
                    stepOrder++;
                    newSteps.Add(step);
                }

                List<Tag> newTags = new();
                foreach (ListViewItem item in lvTags.Items)
                {
                    Tag tag = item.Tag as Tag;

                    Tag? fetchedTag = unitofwork.rManager.GetTagByName(tag.Name);
                    if (fetchedTag != null)
                    {
                        newTags.Add(fetchedTag);
                    }
                    else
                    {
                        newTags.Add(tag);
                    }
                }

                oldRecipe.Name = tbRecipeName.Text;
                oldRecipe.Ingredients = newIngredients;
                oldRecipe.Steps = newSteps;
                oldRecipe.Tags = newTags;
                context.Recipes.Update(oldRecipe);
                context.SaveChanges();
            }

            ((RecipeWindow)this.Owner).LoadDbRecipes();
            this.Owner.Show();
            this.Close();
        }

        // Lägger till en ingrediens i listviewen. Visar endast quantity om det inte är en tom sträng.
        private void btnAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            Ingredient newIngredient = new()
            {
                Name = tbAddIngredient.Text,
                Quantity = tbAddQuantity.Text,
            };

            ListViewItem newItem = new();
            newItem.Content = newIngredient.Name;
            if (newIngredient.Quantity != "")
            {
                newItem.Content += $" x{newIngredient.Quantity}";
            }
            newItem.Tag = newIngredient;
            lvIngredients.Items.Add(newItem);
            tbAddIngredient.Clear();
            tbAddQuantity.Clear();
            btnDelete.Visibility = Visibility.Hidden;
        }

        // Tar bort en ingrediens från listviewen och gömmer delete knappen igen.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            lvIngredients.Items.Remove(lvIngredients.SelectedItem);
            btnDelete.Visibility = Visibility.Hidden;
        }

        // Om man ångrar sig och markerar en ingrediens i listviewen så dyker "Delete ingredient" knappen upp.
        private void lvIngredients_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnDelete.Visibility = Visibility.Visible;
        }

        // När man lägger till ett step i ett nytt recept, så baserar den step order på hur många step det redan finns i listan + 1.
        // Så om det är första steget som läggs till så får den 0+1, dvs 1. Därefter anpassar den sig och fortsätter uppåt.
        private void btnAddStep_Click(object sender, RoutedEventArgs e)
        {
            Step newStep = new()
            {
                Description = tbAddStep.Text,
                Order = lvSteps.Items.Count + 1,
            };

            ListViewItem newItem = new();
            newItem.Content = $"{newStep.Order}) {newStep.Description}";
            newItem.Tag = newStep;
            lvSteps.Items.Add(newItem);
            tbAddStep.Clear();
        }

        // När man trycker add tag så läggs tagen till i listview med en hashtag före.
        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            Tag newTag = new()
            {
                Name = tbAddTag.Text,
            };

            ListViewItem newItem = new();
            newItem.Content = $"#{newTag.Name}";
            newItem.Tag = newTag;
            lvTags.Items.Add(newItem);
            tbAddTag.Clear();
        }
    }
}
