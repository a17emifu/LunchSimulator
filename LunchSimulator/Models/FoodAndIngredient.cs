using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchSimulator.Models
{
    class FoodAndIngredient
    {
        public int ID;
        public int Food_id;
        public int Ingredient_id;

        public void MakeFoodAndIngredient(int food_id, int ingredient_id)
        {
            Food_id = food_id;
            Ingredient_id = ingredient_id;
        }
    }
}
