using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunchSimulator.Models;
using System.Configuration;
using Npgsql;

namespace LunchSimulator.Repositories
{
    class FoodRepo
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;

        #region CREATE
        public static int AddIngredient(Ingredient ingredient)
        {
            string stmt = "INSERT INTO ingredient(name) values(@name) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var command = new NpgsqlCommand(stmt, conn))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("name", ingredient.Name);
                            command.Connection = conn;
                            command.CommandText = stmt;
                            command.Prepare();
                            int id = (int)command.ExecuteScalar();
                            trans.Commit();
                            ingredient.ID = id;
                            return id;
                        }
                        catch (PostgresException)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
        public static int AddFood(Food food)
        {
            string stmt = "INSERT INTO food(name,person_id) values(@name,@person_id) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var command = new NpgsqlCommand(stmt, conn))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("person_id", food.Person_id);
                            command.Parameters.AddWithValue("name", food.Name);
                            command.Connection = conn;
                            command.CommandText = stmt;
                            command.Prepare();
                            int id = (int)command.ExecuteScalar();
                            trans.Commit();
                            food.ID = id;
                            return id;
                        }
                        catch (PostgresException)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
        public static void AddFoodandIngredients(List<FoodAndIngredient> foodAndIngredients) 
        {
            string stmt = "INSERT INTO foodingredient(food_id, ingredient_id) values(@food_id, @ingredient_id) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand())
                        {
                            foreach (var foodandingredient in foodAndIngredients)
                            {
                                command.Parameters.AddWithValue("food_id", foodandingredient.Food_id);
                                command.Parameters.AddWithValue("ingredient_id", foodandingredient.Ingredient_id);
                                command.Connection = conn;
                                command.CommandText = stmt;
                                command.Prepare();
                                int result = (int)command.ExecuteScalar();
                                command.Parameters.Clear();
                            }
                        }
                        trans.Commit();
                    }
                    catch (PostgresException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion
        #region READ
        public static IEnumerable<Ingredient> GetIngredients()
        {
            string stmt = "select id, name from ingredient order by name";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Ingredient ingredient = null;
                List<Ingredient> ingredients = new List<Ingredient>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingredient = new Ingredient
                            {
                                ID = (int)reader["id"],
                                Name = (string)reader["name"],
                            };
                            ingredients.Add(ingredient);
                        }
                    }
                    return ingredients;
                }
            }
        }
        public static Ingredient GetIngredientFromID(int id)
        {
            string stmt = "select id, name from ingredient where id = @id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Ingredient ingredient = null;
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingredient = new Ingredient
                            {
                                ID = (int)reader["id"],
                                Name = (string)reader["name"],
                            };
                        }
                    }
                    return ingredient;
                }
            }
        }
        public static IEnumerable<Food> GetFoods(int person_id)
        {
            string stmt = "select id, name,person_id from food where person_id = @person_id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Food food = null;
                List<Food> foods = new List<Food>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("person_id", person_id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            food = new Food
                            {
                                ID = (int)reader["id"],
                                Name = (string)reader["name"],
                                Person_id = (int)reader["person_id"],
                            };
                            foods.Add(food);
                        }
                    }
                    return foods;
                }
            }
        }
        public static Food GetFoodFromFoodID(int id)
        {
            string stmt = "select id, name, person_id from food where id = @id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Food food= null;

                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            food = new Food
                            {
                                ID = (int)reader["id"],
                                Name = (string)reader["name"],
                                Person_id = (int)reader["person_id"],

                            };

                        }
                    }
                    return food;
                }
            }
        }
        public static IEnumerable<Food> GetFoodsFromFoodList(List<FoodAndIngredient> foodandingredients)
        {
            List<Food> foods = new List<Food>();
            Food food;
            foreach(var foodandingredient in foodandingredients)
            {
                food = new Food();
                food = GetFoodFromFoodID(foodandingredient.Food_id);
                foods.Add(food);
            }

            return foods;
        }
        public static IEnumerable<Ingredient> GetIngredientsFromFoodList(List<FoodAndIngredient> foodandingredients)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            Ingredient ingredient;
            foreach (var foodandingredient in foodandingredients)
            {
                ingredient = new Ingredient();
                ingredient = GetIngredientFromID(foodandingredient.Ingredient_id);
                ingredients.Add(ingredient);
            }
            return ingredients;
        }
        public static List<FoodAndIngredient> GetFoodAndIngredientsFromIngredientID(int ingredient_id)
        {
            string stmt = "select id, food_id,ingredient_id from foodingredient where ingredient_id = @ingredient_id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                FoodAndIngredient foodAndIngredient = null;
                List<FoodAndIngredient> foodandingredients = new List<FoodAndIngredient>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("ingredient_id", ingredient_id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foodAndIngredient = new FoodAndIngredient
                            {
                                ID = (int)reader["id"],
                                Food_id = (int)reader["food_id"],
                                Ingredient_id = (int)reader["ingredient_id"],
                            };
                            foodandingredients.Add(foodAndIngredient);
                        }
                    }
                    return foodandingredients;
                }
            }
        }
        public static List<FoodAndIngredient> GetFoodAndIngredientsFromFoodID(int food_id)
        {
            string stmt = "select id, food_id,ingredient_id from foodingredient where food_id = @food_id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                FoodAndIngredient foodAndIngredient = null;
                List<FoodAndIngredient> foodandingredients = new List<FoodAndIngredient>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("food_id", food_id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foodAndIngredient = new FoodAndIngredient
                            {
                                ID = (int)reader["id"],
                                Food_id = (int)reader["food_id"],
                                Ingredient_id = (int)reader["ingredient_id"],
                            };
                            foodandingredients.Add(foodAndIngredient);
                        }
                    }
                    return foodandingredients;
                }
            }
        }
        #endregion
        #region UPDATE
        #endregion
        #region DELETE
        public static void DeleteIngredient(int id)
        {
            string stmt = "DELETE FROM ingredient WHERE id = @id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteScalar();
                }
            }
        }
        public static void DeleteFood(int id)
        {
            string stmt = "DELETE FROM food WHERE id = @id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteScalar();
                }
            }
        }
        #endregion
    }
}
