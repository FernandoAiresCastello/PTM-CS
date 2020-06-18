using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class Variable
    {
        public string Name { get; private set; }
        public object Value { set; get; }

        public Variable(string name, object value)
        {
            Name = name.Trim().ToUpper();
            Value = value;
        }
    }
}
