using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class ProgramLine
    {
        public int SourceLineNumber { set; get; }
        public int ActualLineNumber { set; get; }
        public string Command { set; get; }
        public CommandParams Params { set; get; }

        public ProgramLine(int sourceLineNumber, int actualLineNumber, string command, CommandParams param)
        {
            SourceLineNumber = sourceLineNumber;
            ActualLineNumber = actualLineNumber;
            Command = command;
            Params = param;
        }

        public override string ToString()
        {
            return ActualLineNumber + ": " + Command + " " + Params;
        }

        public string ToSourceString()
        {
            return SourceLineNumber + ": " + Command + " " + Params;
        }
    }
}
