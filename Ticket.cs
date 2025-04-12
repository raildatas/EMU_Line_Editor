using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELE
{
    public class Ticket : Dictionary<SeatType, int>
    {
        public Ticket(params SeatType[] st) 
        {
            foreach (var t in st)
                Add(t, 0);
        }
        public Ticket(Ticket tk)
        {
            for(int i = 0; i < tk.Count; i++)
                Add(tk.k[i], tk[tk.k[i]]);
        }
        public List<SeatType> k = new List<SeatType>();
        public new void Add(SeatType s, int b)
        {
            k.Add(s);
            base.Add(s, b);
        }
    }
}
