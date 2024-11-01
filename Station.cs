using System;
using System.Collections.Generic;

namespace ELE
{
    public class Station
    {
        public List<int> rails = new List<int>();
        public String name;

        public Station(String name)
        {
            this.name = name;
            rails = new List<int>();
        }
        public void AddRail(int[] index)
        {
            rails.AddRange(index);
        }
    }
}