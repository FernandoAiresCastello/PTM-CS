using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TileGameLib.Engine;
using TileGameLib.File;
using TileGameLib.GameElements;
using TileGameLib.Graphics;

namespace PTM.Engine
{
    public class Machine : GameEngine
    {
        public Program Program { get; private set; }

        private readonly Commands Commands;
        private readonly Stack ExprStack = new Stack();
        private readonly Stack CallStack = new Stack();
        private readonly Variables Vars = new Variables();
        private long Cycle = 0;
        private int ProgramPtr = 0;
        private ObjectMap Map;
        private ObjectPosition TargetObject;
        private int KeyDownHandlerLabel;
        private int KeyUpHandlerLabel;

        public Machine(Program program)
        {
            Program = program;
            ProgramLine firstProgramLine = Program.Lines[0];
            Commands = new Commands(this);

            try
            {
                if (firstProgramLine.Command == "WINDOW.INIT")
                    InitWindow(firstProgramLine.Params);
                else
                    throw new PTMException("WINDOW.INIT expected");
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                throw new PTMException(ex.Message);
            }
        }

        public void Error(string msg)
        {
            Halt(null);
            string sourceLineDesc = "";
            if (Commands.CurrentLine != null)
                sourceLineDesc = "\n\nAt line " + Commands.CurrentLine.ToSourceString();

            ShowErrorMessage(msg + sourceLineDesc);
        }

        public static void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public override void OnStart()
        {
            new Thread(ExecuteProgram).Start();
        }

        private void ExecuteProgram()
        {
            try
            {
                while (Running)
                {
                    if (ProgramPtr >= Program.Lines.Count)
                        throw new PTMException("Program pointer past end of program");

                    ProgramLine line = Program.Lines[ProgramPtr];

                    if (IsExecutable(line))
                    {
                        Commands.Execute(line);
                        Cycle++;
                    }

                    ProgramPtr++;
                }
            }
            catch (PTMException ptmex)
            {
                Error(ptmex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                throw new PTMException(ex.Message);
            }
        }

        public override void OnExecuteCycle()
        {
            // Nothing to do here
        }

        private bool IsExecutable(ProgramLine line)
        {
            return !Program.IsLabel(line.Command);
        }

        public override bool OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
                ShowDebugger(null);

            return true;
        }

        public override bool OnKeyUp(KeyEventArgs e)
        {
            return true;
        }
        
        //=====================================================================
        //  COMMAND MAPPING STARTS HERE
        //=====================================================================

        public void DoNothing(CommandParams param)
        {
            // Spend 1 cycle doing nothing
        }

        public void InitWindow(CommandParams param)
        {
            if (param.Count != 5)
                throw new PTMException("Invalid parameter count");

            try
            {
                string title = param.GetString();
                int width = param.GetNumber();
                int height = param.GetNumber();
                int cols = param.GetNumber();
                int rows = param.GetNumber();

                Window = new GameWindow(this, "", cols, rows);
                Window.Size = new Size(width, height);
                Window.Text = title;
            }
            catch (Exception ex)
            {
                throw new PTMException("Invalid parameter(s)");
            }
        }

        public void ShowDebugger(CommandParams param)
        {
            Debugger.Show();
        }

        public void PrintToDebugger(CommandParams param)
        {
            Debugger.Println(param.GetString());
        }

        public void Halt(CommandParams param)
        {
            Debugger.Println("System halted");
            Stop();
        }

        public void Exit(CommandParams param)
        {
            Exit();
        }

        public void SetWindowTitle(CommandParams param)
        {
            Window.Text = param.GetString();
        }

        public void PushToStack(CommandParams param)
        {
            foreach (string value in param.ToList())
                ExprStack.Push(value.Trim());
        }

        public void PushDuplicateToStack(CommandParams param)
        {
            ExprStack.DuplicateTop();
        }

        public void LoadMap(CommandParams param)
        {
            try
            {
                Map = MapFile.LoadFromRawBytes(param.GetString());
                Window.Graphics.Tileset = Map.Tileset;
                Window.Graphics.Palette = Map.Palette;

                if (MapRenderer != null)
                    MapRenderer.Map = Map;
            }
            catch (DirectoryNotFoundException dnfex)
            {
                throw new PTMException("Map file not found: " + dnfex.Message);
            }
            catch (FileNotFoundException fnfex)
            {
                throw new PTMException("Map file not found: " + fnfex.FileName);
            }
        }

        public void InitMapRenderer(CommandParams param)
        {
            int x = param.GetNumber();
            int y = param.GetNumber();
            int cols = param.GetNumber();
            int rows = param.GetNumber();

            MapRenderer = new MapRenderer(Map, Window.Display, new Rectangle(x, y, cols, rows));
        }

        public void DumpVariables(CommandParams param)
        {
            if (Vars.Count > 0)
            {
                Debugger.Println("--- Variables ---");
                foreach (Variable var in Vars)
                {
                    string name = var.Name;
                    string value = var.Value != null ? var.Value.ToString() : "<null>";
                    Debugger.Println($" {name} = {value}");
                }
            }
            else
            {
                Debugger.Println("--- Variables empty ---");
            }
        }

        public void DumpExprStack(CommandParams param)
        {
            if (ExprStack.Count > 0)
            {
                Debugger.Println("--- Stack ---");
                foreach (string value in ExprStack)
                    Debugger.Println(" " + value);
            }
            else
            {
                Debugger.Println("--- Stack empty ---");
            }
        }

        public void DumpCurrentCycle(CommandParams param)
        {
            Debugger.Println(Cycle);
        }

        public void StoreStackToVariable(CommandParams param)
        {
            Vars.Set(param.GetString(), ExprStack.PopString());
        }

        public void LoadVariableToStack(CommandParams param)
        {
            ExprStack.Push(Vars.GetAsString(param.GetString()));
        }

        public void IncrementStackTop(CommandParams param)
        {
            ExprStack.Push(ExprStack.PopNumber() + 1);
        }

        public void DecrementStackTop(CommandParams param)
        {
            ExprStack.Push(ExprStack.PopNumber() - 1);
        }

        public void AddTop2ValuesOnStack(CommandParams param)
        {
            int a = ExprStack.PopNumber();
            int b = ExprStack.PopNumber();
            ExprStack.Push(a + b);
        }

        public void SubtractTop2ValuesOnStack(CommandParams param)
        {
            int a = ExprStack.PopNumber();
            int b = ExprStack.PopNumber();
            ExprStack.Push(b - a);
        }

        public void MultiplyTop2ValuesOnStack(CommandParams param)
        {
            int a = ExprStack.PopNumber();
            int b = ExprStack.PopNumber();
            ExprStack.Push(a * b);
        }

        public void DivideTop2ValuesOnStack(CommandParams param)
        {
            int divisor = ExprStack.PopNumber();
            if (divisor == 0)
                throw new PTMException("Division by zero");

            int dividend = ExprStack.PopNumber();

            ExprStack.Push(dividend / divisor);
        }

        public void DivideTop2ValuesOnStackPushRemainder(CommandParams param)
        {
            int divisor = ExprStack.PopNumber();
            if (divisor == 0)
                throw new PTMException("Division by zero");

            int dividend = ExprStack.PopNumber();
            Math.DivRem(dividend, divisor, out int remainder);
            ExprStack.Push(remainder);
        }

        private void AssertTargetPosition()
        {
            if (TargetObject == null)
                throw new PTMException("Target position is null");
        }

        private int TryGetLabelLineNumber(string label)
        {
            if (Program.GetLabel(label) == null)
                throw new PTMException("Undefined label");

            return Program.GetLabel(label).LineNumber;
        }

        public void SetTargetPosition(CommandParams param)
        {
            int layer = param.GetNumber();
            int x = param.GetNumber();
            int y = param.GetNumber();

            TargetObject = new ObjectPosition(layer, x, y);
        }

        public void PutObject(CommandParams param)
        {
            AssertTargetPosition();

            GameObject o = new GameObject(new Tile());
            o.Id = param.GetString();
            o.Animation.Clear();
            Map.PutObject(o, TargetObject);
        }

        public void FindObjectById(CommandParams param)
        {
            PositionedObject po = Map.FindObjectById(param.GetString());
            if (po == null)
                throw new PTMException("Object not found with id: " + param);

            TargetObject = po.Position;
        }

        public void AddObjectAnimation(CommandParams param)
        {
            AssertTargetPosition();

            int tileIx = param.GetNumber();
            int tileFgc = param.GetNumber();
            int tileBgc = param.GetNumber();

            GameObject o = Map.GetObject(TargetObject);
            o.Animation.AddFrame(new Tile(tileIx, tileFgc, tileBgc));
        }

        public void SetObjectAnimationFrame(CommandParams param)
        {
            AssertTargetPosition();

            int frame = param.GetNumber();
            int tileIx = param.GetNumber();
            int tileFgc = param.GetNumber();
            int tileBgc = param.GetNumber();

            GameObject o = Map.GetObject(TargetObject);
            o.Animation.SetFrame(frame, new Tile(tileIx, tileFgc, tileBgc));
        }

        public void MoveObjectByDistance(CommandParams param)
        {
            AssertTargetPosition();

            int distLayer = param.GetNumber();
            int distX = param.GetNumber();
            int distY = param.GetNumber();
            ObjectPosition newPosition = new ObjectPosition(TargetObject);
            newPosition.MoveDistance(distX, distY);
            newPosition.AtLayer(newPosition.Layer + distLayer);

            Map.MoveObject(TargetObject, newPosition);
        }

        public void MoveObjectTo(CommandParams param)
        {
            AssertTargetPosition();

            int layer = param.GetNumber();
            int x = param.GetNumber();
            int y = param.GetNumber();
            ObjectPosition newPosition = new ObjectPosition(layer, x, y);

            Map.MoveObject(TargetObject, newPosition);
        }

        public void SetKeyDownHandler(CommandParams param)
        {
            KeyDownHandlerLabel = TryGetLabelLineNumber(param.GetString());
        }

        public void SetKeyUpHandler(CommandParams param)
        {
            KeyUpHandlerLabel = TryGetLabelLineNumber(param.GetString());
        }

        public void CallSubroutineAtLabel(CommandParams param)
        {
            CallStack.Push(ProgramPtr);
            ProgramPtr = TryGetLabelLineNumber(param.GetString());
        }

        public void ReturnFromSubroutine(CommandParams param)
        {
            ProgramPtr = CallStack.PopNumber();
        }

        public void GoToLabel(CommandParams param)
        {
            ProgramPtr = TryGetLabelLineNumber(param.GetString());
        }
    }
}
