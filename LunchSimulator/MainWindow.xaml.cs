using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LunchSimulator.Models;
using Npgsql;
using static LunchSimulator.Repositories.FoodRepo;
using static LunchSimulator.Repositories.PersonRepo;

namespace LunchSimulator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Show_Ingredients();
            comboPeople.ItemsSource = Getpeople();
            
        }
        private void Show_Ingredients()
        {
            listIngredients.ItemsSource = GetIngredients();
            listIngredients1.ItemsSource = GetIngredients();
            listIngredients2.ItemsSource = GetIngredients();
        }

        private void Register_Person(string name)
        {
            Person person = new Person();
            if (name == "")
            {
                name = "名無しの坊や";
            }
            person.RegisterPerson(name);
            AddPerson(person);

            MessageBox.Show("登録したよ！");
            boxRegisterPerson.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Register_Person(boxRegisterPerson.Text);
            comboPeople.ItemsSource = Getpeople();
        }
        private void Delete_person(Person person)
        {
            DeletePerson(person.ID);
            comboPeople.ItemsSource = Getpeople();

            MessageBox.Show("削除したよ！");
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Person person = comboPeople.SelectedItem as Person;
            if (person != null)
            {
                try
                {
                    Delete_person(person);
                }
                catch (PostgresException ex)
                {
                    if (ex.ErrorCode == -2147467259)
                    {
                        MessageBox.Show($"{person.Name} har recept! Om du vill ta bort själv måste du ta bort recept först.");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                        
                }
            }
           
        }
        private void Register_Ingredient(string name)
        {
            Ingredient ingredient = new Ingredient();
            ingredient.RegisterIngredient(name);
            AddIngredient(ingredient);
            Show_Ingredients();
            boxIngredient.Text = "";

            //MessageBox.Show($"{name}を追加したよ！");
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (boxIngredient.Text == "")
            {
                MessageBox.Show("ちゃんと材料いれて！");
            }
            else
            {
                Register_Ingredient(boxIngredient.Text);
            }
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Ingredient ingredient = listIngredients.SelectedItem as Ingredient;
            Delete_Ingredient(ingredient);
        }
        private void Delete_Ingredient(Ingredient ingredient)
        {
            DeleteIngredient(ingredient.ID);
            Show_Ingredients();
            MessageBox.Show($"{ingredient.Name}を削除したよ！");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (boxFoodname.Text != "料理の名前")
            {
                txtFoodname.Text = boxFoodname.Text;
            }
            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Ingredient ingredient = listIngredients1.SelectedItem as Ingredient;
            if (ingredient != null)
            {
                Add_Ingredient_To_List(ingredient);
            }
        }
        private void Add_Ingredient_To_List(Ingredient ingredient)
        {
            listIngredientsForRegisterFood.Items.Add(ingredient);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Ingredient ingredient = listIngredientsForRegisterFood.SelectedItem as Ingredient;
            if (ingredient != null)
            {
                Delete_Ingredient_From_List(ingredient);
            }
        }
        private void Delete_Ingredient_From_List(Ingredient ingredient)
        {
            listIngredientsForRegisterFood.Items.Remove(ingredient);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Person person = comboPeople.SelectedItem as Person;
            List<Ingredient> ingredients = listIngredientsForRegisterFood.Items.Cast<Ingredient>().ToList();
            if ((person != null)&& (ingredients != null))
            {
                Food food =
                Register_Food(person, txtFoodname.Text);
                Register_FoodAndIngredient(ingredients, food);
                Show_Foods(person);
                Reset_UI_RegisterFood();

            }
        }
        private Food Register_Food(Person person, string foodname)
        {
            Food food = new Food
            {
                Name = foodname,
                Person_id = person.ID
            };
            AddFood(food);
            return food;
        }

        private void Register_FoodAndIngredient(List<Ingredient> ingredients, Food food)
        {
            List<FoodAndIngredient> foodAndIngredients = new List<FoodAndIngredient>();
            foreach(var ingredient in ingredients)
            {
                FoodAndIngredient foodAndIngredient = new FoodAndIngredient();
                foodAndIngredient.MakeFoodAndIngredient(food.ID, ingredient.ID);

                foodAndIngredients.Add(foodAndIngredient);
            }
            AddFoodandIngredients(foodAndIngredients);
        }

        private void comboPeople_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Person person = comboPeople.SelectedItem as Person;
            listFoods.ItemsSource = null;
            if (person != null)
            {
                Show_Foods(person);
            }
        }
        private void Show_Foods(Person person)
        {
            listFoods.ItemsSource = GetFoods(person.ID);
            listFoods1.ItemsSource = GetFoods(person.ID);
        }
        private void Reset_UI_RegisterFood()
        {
            boxFoodname.Text = "";
            txtFoodname.Text = "";
            listIngredientsForRegisterFood.ItemsSource = null;
            listIngredientsForRegisterFood.Items.Clear();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Person person = comboPeople.SelectedItem as Person;
            Food food = listFoods.SelectedItem as Food;
            if (food != null)
            {
                DeleteFood(food.ID);
                Show_Foods(person);
            }
            
        }

        private void listIngredients2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ingredient ingredient = listIngredients2.SelectedItem as Ingredient;
            if(ingredient != null)
            {
                List<FoodAndIngredient> foodAndIngredients = GetFoodAndIngredientsFromIngredientID(ingredient.ID);
                listFoods1.ItemsSource= GetFoodsFromFoodList(foodAndIngredients);

                lblFoodName.Content = "";
                listIngredientsForfood.ItemsSource = null;
            }
        }

        private void listFoods1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Food food = listFoods1.SelectedItem as Food;
            if (food != null)
            {
                lblFoodName.Content = food.Name;
                List<FoodAndIngredient> foodAndIngredients =  GetFoodAndIngredientsFromFoodID(food.ID);
                listIngredientsForfood.ItemsSource= GetIngredientsFromFoodList(foodAndIngredients);
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            Person person =
            GetPersonRandom();
            nameblock.Text = person.Name;
        }
    }
}
