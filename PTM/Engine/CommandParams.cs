﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class CommandParams
    {
        public int Count => Params.Count;
        public List<string> ToList() => Params;

        private readonly List<string> Params = new List<string>();
        private int CurrentIndex = 0;

        private CommandParams()
        {
        }

        public CommandParams(string paramList)
        {
            if (paramList != null)
                Params.AddRange(paramList.Split(','));
        }

        public CommandParams(CommandParams other)
        {
            Params.AddRange(other.Params);
        }

        public override string ToString()
        {
            StringBuilder paramString = new StringBuilder();

            for (int i = 0; i < Count; i++)
                paramString.Append(i < Count - 1 ? Params[i] + ", " : Params[i]);

            return paramString.ToString();
        }

        public string GetString()
        {
            if (CurrentIndex >= Count)
                throw new PTMException("Invalid parameter count");

            return Params[CurrentIndex++];
        }

        public int GetNumber()
        {
            return int.Parse(GetString());
        }
    }
}
