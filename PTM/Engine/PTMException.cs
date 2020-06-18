using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class PTMException : Exception
    {
        public PTMException(string message) : base(message)
        {
        }
    }
}
