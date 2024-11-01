using System;

namespace ELE
{
    public class ListItem
    {
        public int before;
        public int after;
        public int line;
        public int beforestop;
        public int afterstop;
        public int earlytime;
        public RunMode mode;
        public Boolean beforeteg;
        public Boolean afterteg;
        public ListItem(int before, int beforestop, int line, RunMode mode, int after, int afterstop, int earlytime, bool beforeteg, bool afterteg)
        {
            this.before = before;
            this.line = line;
            this.after = after;
            this.mode = mode;
            this.beforestop = beforestop;
            this.afterstop = afterstop;
            this.earlytime = earlytime;
            this.beforeteg = beforeteg;
            this.afterteg = afterteg;
        }
    }
}