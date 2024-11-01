using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELE
{
    internal struct Share
    {
        public static List<EMU> emus;
        public static List<Rail> rails;
        public static List<Station> stations;
        public static Dictionary<char, List<int>> lists;
        public static bool delayformflag = false;
        public static int delayformdata1 = 0;
        public static Dictionary<string, string> settings = new Dictionary<string, string>();
    }

    public struct DataFiles
    {
        public static string[] emuDatas;
        public static string[] railDatas;
    }
}