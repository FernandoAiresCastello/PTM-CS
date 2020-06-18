using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly Program Program;
        private readonly Commands Commands;
        private readonly Stack Stack = new Stack();
        private readonly Variables Vars = new Variables();
        private long Cycle = 0;
        private int ProgramPtr = 0;
        private ObjectMap Map;
        private ObjectPosition Target;

        public Machine(Program program)
        {
            Program = program;
            Commands = new Commands(this);

            ProgramLine firstProgramLine = Program.Lines[0];

            try
            {
                if (firstProgramLine.Command == "WINDOW")
                    InitWindow(firstProgramLine.Params);
                else
                    throw new PTMException("WINDOW expected");
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
            // Nothing to do here yet
        }

        public override void OnExecuteCycle()
        {
            try
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

        public void DoNothing(string param)
        {
            // Spend 1 cycle doing nothing
        }

        public void InitWindow(string param)
        {
            try
            {
                string[] args = param.Split(',');
                int width = int.Parse(args[0].Trim());
                int height = int.Parse(args[1].Trim());
                int cols = int.Parse(args[2].Trim());
                int rows = int.Parse(args[3].Trim());

                Window = new GameWindow(this, "", cols, rows);
                Window.Size = new Size(width, height);
            }
            catch
            {
                throw new PTMException("Invalid WINDOW setup");
            }
        }

        public void ShowDebugger(string param)
        {
            Debugger.Show();
        }

        public void PrintToDebugger(string param)
        {
            Debugger.Print(param);
        }

        public void Halt(string param)
        {
            Stop();
        }

        public void Exit(string param)
        {
            Exit();
        }

        public void SetWindowTitle(string param)
        {
            Window.Text = param;
        }

        public void PushToStack(string param)
        {
            string[] args = param.Split(',');
            foreach (string arg in args)
                Stack.Push(arg.Trim());
        }

        public void PushDuplicateToStack(string param)
        {
            Stack.DuplicateTop();
        }

        public void LoadMap(string param)
        {
            try
            {
                Map = MapFile.LoadFromRawBytes(param);
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

        public void InitMapRenderer(string param)
        {
            string[] args = param.Split(',');
            int x = int.Parse(args[0]);
            int y = int.Parse(args[1]);
            int cols = int.Parse(args[2]);
            int rows = int.Parse(args[3]);

            MapRenderer = new MapRenderer(Map, Window.Display, new Rectangle(x, y, cols, rows));
        }

        public void DumpVariables(string param)
        {
            if (Vars.Count > 0)
            {
                Debugger.Print("--- Variables ---");
                foreach (Variable var in Vars)
                {
                    string name = var.Name;
                    string value = var.Value != null ? var.Value.ToString() : "<null>";
                    Debugger.Print($" {name} = {value}");
                }
            }
            else
            {
                Debugger.Print("--- Variables empty ---");
            }
        }

        public void DumpStack(string param)
        {
            if (Stack.Count > 0)
            {
                Debugger.Print("--- Stack ---");
                foreach (string value in Stack)
                    Debugger.Print(" " + value);
            }
            else
            {
                Debugger.Print("--- Stack empty ---");
            }
        }

        public void StoreStackToVariable(string param)
        {
            Vars.Set(param, Stack.PopString());
        }

        public void LoadVariableToStack(string param)
        {
            Stack.Push(Vars.GetAsString(param));
        }

        public void IncrementStackTop(string param)
        {
            Stack.Push(Stack.PopNumber() + 1);
        }

        public void DecrementStackTop(string param)
        {
            Stack.Push(Stack.PopNumber() - 1);
        }

        public void AddTop2ValuesOnStack(string param)
        {
            int a = Stack.PopNumber();
            int b = Stack.PopNumber();
            Stack.Push(a + b);
        }

        public void SubtractTop2ValuesOnStack(string param)
        {
            int a = Stack.PopNumber();
            int b = Stack.PopNumber();
            Stack.Push(b - a);
        }

        public void MultiplyTop2ValuesOnStack(string param)
        {
            int a = Stack.PopNumber();
            int b = Stack.PopNumber();
            Stack.Push(a * b);
        }

        public void DivideTop2ValuesOnStack(string param)
        {
            int divisor = Stack.PopNumber();
            if (divisor == 0)
                throw new PTMException("Division by zero");

            int dividend = Stack.PopNumber();

            Stack.Push(dividend / divisor);
        }

        public void DivideTop2ValuesOnStackPushRemainder(string param)
        {
            int divisor = Stack.PopNumber();
            if (divisor == 0)
                throw new PTMException("Division by zero");

            int dividend = Stack.PopNumber();
            Math.DivRem(dividend, divisor, out int remainder);
            Stack.Push(remainder);
        }

        public void SetTargetPosition(string param)
        {
            string[] args = param.Split(',');
            int layer = int.Parse(args[0].Trim());
            int x = int.Parse(args[1].Trim());
            int y = int.Parse(args[2].Trim());

            Target = new ObjectPosition(layer, x, y);
        }

        public void FindObjectById(string param)
        {
            PositionedObject po = Map.FindObjectById(param);
            if (po == null)
                throw new PTMException("Object not found with id: " + param);

            Target = po.Position;
        }

        public void AddObjectAnimation(string param)
        {
            if (Target == null)
                throw new PTMException("Target position is null");

            string[] args = param.Split(',');
            int tileIx = int.Parse(args[0].Trim());
            int tileFgc = int.Parse(args[1].Trim());
            int tileBgc = int.Parse(args[2].Trim());

            GameObject o = Map.GetObject(Target);
            o.Animation.AddFrame(new Tile(tileIx, tileFgc, tileBgc));
        }

        public void PutObject(string param)
        {
            if (Target == null)
                throw new PTMException("Target position is null");

            GameObject o = new GameObject(new Tile());
            o.Id = param;
            o.Animation.Clear();
            Map.PutObject(o, Target);
        }
    }
}
