using System;
using System.Collections.Generic;

namespace ELE
{
    public class EMU
    {
        public double ast;
        public List<List<Train>> trains;
        public String name;
        public List<String> versions;
        public int speed;

        public EMU(double ast, String name, List<List<Train>> trains, List<String> ver, int speed)
        {
            this.ast = ast;
            this.trains = trains;
            this.name = name;
            versions = ver;
            this.speed = speed;
        }
    }
}