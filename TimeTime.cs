using System;

namespace ELE
{
    public class TimeTime
    {
        public int arriveTime;
        public int deparTime;
        public Boolean teg;
        public TimeTime(int arriveTime, int deparTime, Boolean teg)
        {
            this.arriveTime = arriveTime;
            this.deparTime = deparTime;
            this.teg = teg;
        }
    }
}