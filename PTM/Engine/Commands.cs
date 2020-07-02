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
            Cmd["DEBUG.VARS.DUMP"] = Machine.DumpVariables;
            Cmd["DEBUG.STK.DUMP"] = Machine.DumpExprStack;
            Cmd["DEBUG.CYCLE"] = Machine.DumpCurrentCycle;
            // System
            Cmd["SYS.HALT"] = Machine.Halt;
            Cmd["SYS.EXIT"] = Machine.Exit;
            // Window
            Cmd["WINDOW.INIT"] = Machine.DoNothing;
            // Expression stack
            Cmd["STK.PUSH"] = Machine.PushToStack;
            Cmd["STK.DUP"] = Machine.PushDuplicateToStack;
            Cmd["STK.STORE"] = Machine.StoreStackToVariable;
            Cmd["STK.LOAD"] = Machine.LoadVariableToStack;
            // Program branching
            Cmd["CALL"] = Machine.CallLabel;
            Cmd["RET"] = Machine.ReturnFromSubroutine;
            Cmd["GOTO"] = Machine.GoToLabel;
            // Variables
            Cmd["VAR"] = Machine.SetVariable;
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
            Cmd["OBJ.MOVE.DIST"] = Machine.MoveObjectByDistance;
            Cmd["OBJ.MOVE.TO"] = Machine.MoveObjectTo;
            // Keyboard
            Cmd["IF.KEY.CALL"] = Machine.IfKeyPressedCall;
            Cmd["IF.KEY.GOTO"] = Machine.IfKeyPressedGoto;
        }

        public ProgramLine CurrentLine { get; private set; }

        private readonly Machine Machine;
        private readonly Dictionary<string, Action<CommandParams>> Cmd;
        private readonly Variables Vars;

        public Commands(Machine machine)
        {
            Machine = machine;
            CurrentLine = machine.Program.Lines[0];
            Cmd = new Dictionary<string, Action<CommandParams>>();
            Vars = machine.Vars;
            Init();
        }

        public void Execute(ProgramLine line)
        {
            CurrentLine = line;
            string command = line.Command;

            if (!Cmd.ContainsKey(command))
                throw new PTMException($"Unrecognized command: {command}");

            Cmd[command](new CommandParams(Vars, line.Params));
        }
    }
}
