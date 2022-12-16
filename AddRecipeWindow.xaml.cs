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

        // Om man ångrar sig och markerar en ingrediens i listviewen så dyker "Delete ingredient" knappen upp.
        private void lvIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnDelete.Visibility = Visibility.Visible;
        }

        // Tar bort en ingrediens från listviewen och gömmer delete knappen igen.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            lvIngredients.Items.Remove(lvIngredients.SelectedItem);
            btnDelete.Visibility = Visibility.Hidden;
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

        // Här sparas all inmatad information tillsammans i ett recept.
        // Men först är det 3 checks som ser till att man skriver in ett recipe name, lägger till minst 1 ingrediens samt step.
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

                            // Funktionen att spara tags och kolla efter nuvarande matchningar var klurigt.
                            // Fick ta hjälp av andra i klassen för att få detta och fungera korrekt.
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

        // Avbryter receptet och går tillbaka till ägaren, vilket i detta fallet är RecipeWindow.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        // Skapar en ingredienslista och lägger till alla items i listviewen till den.
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

        // Skapar en steplista som har en counter som börjar på 1 och går +1 för varje step som läggs till.
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

        // Skapar en taglista med alla tags i listviewen.
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
