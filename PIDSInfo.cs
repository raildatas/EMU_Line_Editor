using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELE
{
    public class PIDSInfo
    {
        public bool isStop;
        public DateTime arriveTime;
        public short delayTime;
        public string name;

        public PIDSInfo(string str)
        {
            this.isStop = str.Contains("|");
            string[] strs = str.Split('|');
            this.name = strs[0];
            if (isStop)
            {
                arriveTime = new DateTime(1, 1, 1, int.Parse(strs[1].Substring(0, 2)), int.Parse(strs[1].Substring(2, 2)), 0);
                if (strs[1].Contains(":"))
                    this.delayTime = short.Parse(strs[1].Split(':')[1]);
                else
                    this.delayTime = 0;
            }
        }
    }
}
