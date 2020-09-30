using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchSimulator.Models
{
    class Food
    {
        public int ID;
        public string Name;
        public int Person_id;

       /* public void RegisterFood (string name, int person_id)
        {
            Name = name;
            Person_id = person_id;
        }*/
        public override string ToString()
        {
            return Name;
        }
    }
}
