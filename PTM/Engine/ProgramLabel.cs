using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class ProgramLabel
    {
        public string Label { set; get; }
        public int LineNumber { set; get; }

        public ProgramLabel(string label, int number)
        {
            Label = label;
            LineNumber = number;
        }

        public override string ToString()
        {
            return LineNumber + " " + Label;
        }
    }
}
