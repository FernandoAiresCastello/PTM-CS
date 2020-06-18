using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class Variables : IEnumerable<Variable>
    {
        public int Count => VariableList.Count;

        private readonly List<Variable> VariableList;

        public Variables()
        {
            VariableList = new List<Variable>();
        }

        public void Clear()
        {
            VariableList.Clear();
        }

        public string GetAsString(string name)
        {
            return Get(name).Value.ToString();
        }

        public int GetAsNumber(string name)
        {
            string value = GetAsString(name);
            bool ok = int.TryParse(value, out int number);
            if (!ok)
                throw new PTMException("Not a number: " + value);

            return number;
        }

        public void Set(string name, object value)
        {
            if (Contains(name))
                Update(name, value);
            else
                VariableList.Add(new Variable(name, value));
        }

        public bool Contains(string name)
        {
            return Find(name) != null;
        }

        private Variable Get(string name)
        {
            Variable variable = Find(name);
            if (variable == null)
                throw new PTMException("Undefined variable: " + name);

            return variable;
        }

        private void Update(string name, object value)
        {
            Find(name).Value = value;
        }

        private Variable Find(string name)
        {
            return VariableList.Find((var) =>
                var.Name == name.Trim().ToUpper());
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            return ((IEnumerable<Variable>)VariableList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Variable>)VariableList).GetEnumerator();
        }
    }
}
