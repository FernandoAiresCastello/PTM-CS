using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTM.Engine
{
    public class Commands
    {
        private void Init()
        {
            // Non-executable
            Cmd["NOP"] = Machine.DoNothing;
            Cmd["WINDOW"] = Machine.DoNothing;
            // Debugging
            Cmd["DEBUG"] = Machine.ShowDebugger;
            Cmd["LOG"] = Machine.PrintToDebugger;
            Cmd["VDUMP"] = Machine.DumpVariables;
            Cmd["SDUMP"] = Machine.DumpStack;
            // System
            Cmd["HALT"] = Machine.Halt;
            Cmd["EXIT"] = Machine.Exit;
            Cmd["TITLE"] = Machine.SetWindowTitle;
            // Stack, Variables
            Cmd["PUSH"] = Machine.PushToStack;
            Cmd["DUP"] = Machine.PushDuplicateToStack;
            Cmd["STORE"] = Machine.StoreStackToVariable;
            Cmd["LOAD"] = Machine.LoadVariableToStack;
            // Arithmetic
            Cmd["INC"] = Machine.IncrementStackTop;
            Cmd["DEC"] = Machine.DecrementStackTop;
            Cmd["ADD"] = Machine.AddTop2ValuesOnStack;
            Cmd["SUB"] = Machine.SubtractTop2ValuesOnStack;
            Cmd["MUL"] = Machine.MultiplyTop2ValuesOnStack;
            Cmd["DIV"] = Machine.DivideTop2ValuesOnStack;
            Cmd["MOD"] = Machine.DivideTop2ValuesOnStackPushRemainder;
            // Maps
            Cmd["LDMAP"] = Machine.LoadMap;
            Cmd["MAPR"] = Machine.InitMapRenderer;
            // GameObjects
            Cmd["POS"] = Machine.SetTargetPosition;
            Cmd["FIND"] = Machine.FindObjectById;
            Cmd["PUT"] = Machine.PutObject;
            Cmd["ANIM"] = Machine.AddObjectAnimation;
        }

        public ProgramLine CurrentLine { get; private set; }

        private readonly Machine Machine;
        private readonly Dictionary<string, Action<string>> Cmd;

        public Commands(Machine machine)
        {
            Machine = machine;
            Cmd = new Dictionary<string, Action<string>>();
            Init();
        }

        public void Execute(ProgramLine line)
        {
            CurrentLine = line;
            string command = line.Command;
            string param = line.Params;

            if (!Cmd.ContainsKey(command))
                throw new PTMException($"Unrecognized command: {command}");

            Cmd[command](param);
        }
    }
}
