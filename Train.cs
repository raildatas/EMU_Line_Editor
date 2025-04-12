using System;
using System.Collections.Generic;

namespace ELE
{
    public class Train
    {
        public Dictionary<SeatType, Byte> dic;
        public List<SeatType> st;
        public byte number;

        public Train(List<Byte> person, List<SeatType> seatTypes, Byte number)
        {
            dic = new Dictionary<SeatType, Byte>();
            st = new List<SeatType>();
            if (seatTypes.Count != person.Count)
            {
                //throw new Exception("对应的席位无法匹配到对应的人数。");
            }
            for (int i = 0; i < seatTypes.Count; i++)
            {
                dic.Add(seatTypes[(i)], person[(i)]);
                st.Add(seatTypes[i]);
            }
            this.number = number;
        }
    }
}