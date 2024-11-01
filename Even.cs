using System;

namespace ELE
{
    public class Even
    {
        public Date date;
        public int year;
        public String content;

        public Even(int year, int month, int day, String content)
        {
            date = new Date(day, month);
            this.year = year;
            this.content = content;
        }

        public int IsNow()
        {
            DateTime now = DateTime.Now;
            int y = now.Year;
            int m = now.Month;
            int d = now.Day;
            if ((m == date.month) && (d == date.day))
                return y - year;
            else
                return -1;
        }
    }
}