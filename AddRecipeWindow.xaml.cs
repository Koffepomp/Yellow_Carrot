using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Yellow_Carrot.Data;
using Yellow_Carrot.Managers;
using Yellow_Carrot.Models;

namespace Yellow_Carrot
{
    /// <summary>
    /// Interaction logic for AddRecipeWindow.xaml
    /// </summary>
    public partial class AddRecipeWindow : Window
    {
        private int loginId;
        public AddRecipeWindow(int loginId)
        {
            InitializeComponent();
            this.loginId = loginId;
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
        private void lvIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDelete.Visibility = Visibility.Visible;
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            lvIngredients.Items.Remove(lvIngredients.SelectedItem);
            btnDelete.Visibility = Visibility.Hidden;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbRecipeName.Text.Length > 0)
            {
                if (lvIngredients.Items.Count > 0)
                {
                    if (lvSteps.Items.Count > 0)
                    {
                        using (RecipeDbContext context = new())
                        {
                            UnitOfWork unitofwork = new(context);
                            Recipe newRecipe = new()
                            {
                                Name = tbRecipeName.Text,
                                UserId = loginId,
                                Ingredients = CreateIngredientsList(),
                                Steps = CreateStepList(),
                            };

                            foreach (Tag tag in CreateTagList())
                            {
                                Tag? fetchedTag = unitofwork.rManager.GetTagByName(tag.Name);
                                if (fetchedTag != null)
                                {
                                    newRecipe.Tags.Add(fetchedTag);
                                }
                                else
                                {
                                    newRecipe.Tags.Add(tag);
                                }
                            }

                            unitofwork.rManager.CreateNewRecipe(newRecipe);
                            context.SaveChanges();
                        }
                        ((RecipeWindow)this.Owner).LoadDbRecipes();
                        this.Owner.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("A recipe must have at least 1 step included.");
                    }
                }
                else
                {
                    MessageBox.Show("A recipe must contain at least 1 ingredient.");
                }
            }
            else
            {
                MessageBox.Show("Please enter a name for the new recipe.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private List<Ingredient> CreateIngredientsList()
        {
            List<Ingredient> newIngredientList = new();
            foreach (ListViewItem item in lvIngredients.Items)
            {
                Ingredient ingredient = item.Tag as Ingredient;
                newIngredientList.Add(ingredient);
            }
            return newIngredientList;
        }

        private List<Step> CreateStepList()
        {
            List<Step> newStepList = new();
            int stepOrder = 1;

            foreach (ListViewItem item in lvSteps.Items)
            {
                Step step = item.Tag as Step;
                step.Order = stepOrder;
                stepOrder++;
                newStepList.Add(step);
            }
            return newStepList;
        }

        private List<Tag> CreateTagList()
        {
            List<Tag> newTagList = new();
            foreach (ListViewItem item in lvTags.Items)
            {
                Tag tag = item.Tag as Tag;
                newTagList.Add(tag);
            }
            return newTagList;
        }

    }
}
