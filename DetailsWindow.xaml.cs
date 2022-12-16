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


        private void LoadRecipeData()
        {
            //körs bara 1 gång när rutan öppnas
            using (RecipeDbContext context = new())
            {
                UnitOfWork unitofwork = new(context);
                Recipe currentRecipe = unitofwork.rManager.GetSpecificRecipe(currentRecipeId);
                tbRecipeName.Text = currentRecipe.Name;

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

                foreach (Step step in currentRecipe.Steps)
                {
                    ListViewItem oldItem = new();
                    oldItem.Content = $"{step.Order}) {step.Description}";
                    oldItem.Tag = step;
                    lvSteps.Items.Add(oldItem);
                }

                // Här ska rätt tags ladda till receptet
                foreach (Tag tag in currentRecipe.Tags)
                {
                    ListViewItem oldItem = new();
                    oldItem.Content = $"#{tag.Name}";
                    oldItem.Tag = tag;
                    lvTags.Items.Add(oldItem);
                }
            }
        }
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

                // Recipe name, disable unlock button and enable save button
                tbRecipeName.IsEnabled = true;
                btnUnlock.IsEnabled = false;
                btnSave.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("You can't edit this recipe!");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // save boxes in db
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            lvIngredients.Items.Remove(lvIngredients.SelectedItem);
            btnDelete.Visibility = Visibility.Hidden;
        }

        private void lvIngredients_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnDelete.Visibility = Visibility.Visible;
        }

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
