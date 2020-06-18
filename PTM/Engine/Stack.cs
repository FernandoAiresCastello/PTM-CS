using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class Stack : IEnumerable<string>
    {
        public int Count => PrivateStack.Count;

        private readonly Stack<string> PrivateStack;

        public Stack()
        {
            PrivateStack = new Stack<string>();
        }

        public void Push(object o)
        {
            PrivateStack.Push(o?.ToString());
        }

        public string PopString()
        {
            if (Count <= 0)
                throw new PTMException("Empty stack");

            return PrivateStack.Pop();
        }

        public int PopNumber()
        {
            string top = PopString();

            if (top.StartsWith("0x"))
                return Convert.ToInt32(top.Substring(2), 16);
            else if (top.StartsWith("0b"))
                return Convert.ToInt32(top.Substring(2), 2);
            else
                return int.Parse(top);
        }

        public void DuplicateTop()
        {
            if (Count > 0)
                PrivateStack.Push(PrivateStack.Peek());
            else
                throw new PTMException("Empty stack");
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)PrivateStack).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)PrivateStack).GetEnumerator();
        }
    }
}
