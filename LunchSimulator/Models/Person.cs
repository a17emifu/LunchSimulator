using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchSimulator.Models
{
    class Person
    {
        public int ID;
        public string Name;

        public void RegisterPerson(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
