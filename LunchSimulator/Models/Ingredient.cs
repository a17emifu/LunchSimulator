using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchSimulator.Models
{
    class Ingredient
    {
        public int ID;
        public string Name;

        public void RegisterIngredient(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
