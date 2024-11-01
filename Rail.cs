using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELE
{
    public class Rail
    {
        public JArray locations;
        public String name;
        public List<String> stations;
        public String speeds;
        public List<Double> lengths;
    }
}