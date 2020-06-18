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
            // Special
            Cmd["NOP"] = Machine.DoNothing;
            // Debugging
            Cmd["DEBUG.SHOW"] = Machine.ShowDebugger;
            Cmd["DEBUG.PRINT"] = Machine.PrintToDebugger;
            Cmd["DEBUG.PRINTLN"] = Machine.PrintLineToDebugger;
            Cmd["DEBUG.VARS.DUMP"] = Machine.DumpVariables;
            Cmd["DEBUG.STACK.DUMP"] = Machine.DumpStack;
            // System
            Cmd["SYS.HALT"] = Machine.Halt;
            Cmd["SYS.EXIT"] = Machine.Exit;
            // Window
            Cmd["WINDOW.INIT"] = Machine.DoNothing;
            Cmd["WINDOW.TITLE"] = Machine.SetWindowTitle;
            // Stack
            Cmd["STK.PUSH"] = Machine.PushToStack;
            Cmd["STK.DUP"] = Machine.PushDuplicateToStack;
            Cmd["STK.STORE"] = Machine.StoreStackToVariable;
            Cmd["STK.LOAD"] = Machine.LoadVariableToStack;
            // Math
            Cmd["MATH.INC"] = Machine.IncrementStackTop;
            Cmd["MATH.DEC"] = Machine.DecrementStackTop;
            Cmd["MATH.ADD"] = Machine.AddTop2ValuesOnStack;
            Cmd["MATH.SUB"] = Machine.SubtractTop2ValuesOnStack;
            Cmd["MATH.MUL"] = Machine.MultiplyTop2ValuesOnStack;
            Cmd["MATH.DIV"] = Machine.DivideTop2ValuesOnStack;
            Cmd["MATH.MOD"] = Machine.DivideTop2ValuesOnStackPushRemainder;
            // Map
            Cmd["MAP.LOAD"] = Machine.LoadMap;
            // Map view
            Cmd["MAPVIEW.INIT"] = Machine.InitMapRenderer;
            // Game object
            Cmd["OBJ.POS"] = Machine.SetTargetPosition;
            Cmd["OBJ.FIND"] = Machine.FindObjectById;
            Cmd["OBJ.PUT"] = Machine.PutObject;
            Cmd["OBJ.ANIM.ADD"] = Machine.AddObjectAnimation;
            Cmd["OBJ.ANIM.SET"] = Machine.SetObjectAnimationFrame;
        }

        public ProgramLine CurrentLine { get; private set; }

        private readonly Machine Machine;
        private readonly Dictionary<string, Action<CommandParams>> Cmd;

        public Commands(Machine machine)
        {
            Machine = machine;
            CurrentLine = machine.Program.Lines[0];
            Cmd = new Dictionary<string, Action<CommandParams>>();
            Init();
        }

        public void Execute(ProgramLine line)
        {
            CurrentLine = line;
            string command = line.Command;

            if (!Cmd.ContainsKey(command))
                throw new PTMException($"Unrecognized command: {command}");

            Cmd[command](line.Params);
        }
    }
}
