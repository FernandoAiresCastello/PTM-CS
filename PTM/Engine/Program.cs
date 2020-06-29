using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTM.Engine
{
    public class Program
    {
        public List<ProgramLine> Lines { private set; get; }
        public List<ProgramLabel> Labels { private set; get; }

        public Program(string filePath)
        {
            try
            {
                Load(filePath);
                Environment.CurrentDirectory = Path.GetDirectoryName(filePath);
            }
            catch (FileNotFoundException fnfex)
            {
                Machine.ShowErrorMessage("Program file not found:\n\n" + fnfex.FileName);
                throw;
            }
            catch (Exception ex)
            {
                Machine.ShowErrorMessage("Error loading program file:\n\n" + ex.Message);
                throw;
            }

            FindLabels();
        }

        private void Load(string filename)
        {
            Lines = new List<ProgramLine>();
            string[] source = File.ReadAllLines(filename);
            int sourceLineNumber = 1;
            int actualLineNumber = 0;

            foreach (string line in source)
            {
                string trimmedLine = line.Trim();

                if (!IsEmptyLine(trimmedLine) && !IsCommentLine(trimmedLine))
                {
                    int delimiterIndex = trimmedLine.IndexOf(' ');
                    string command = null;
                    string param = null;

                    if (delimiterIndex >= 0)
                    {
                        command = trimmedLine.Substring(0, delimiterIndex).Trim();
                        param = trimmedLine.Substring(delimiterIndex).Trim();
                    }
                    else
                    {
                        command = trimmedLine;
                    }

                    Lines.Add(new ProgramLine(sourceLineNumber, actualLineNumber, command.ToUpper(), new CommandParams(param)));
                    actualLineNumber++;
                }

                sourceLineNumber++;
            }
        }

        private void FindLabels()
        {
            Labels = new List<ProgramLabel>();

            foreach (ProgramLine line in Lines)
            {
                if (IsLabel(line.Command))
                {
                    Labels.Add(new ProgramLabel(line.Command, line.ActualLineNumber));
                }
            }
        }

        public ProgramLabel GetLabel(string labelText)
        {
            foreach (ProgramLabel label in Labels)
            {
                if (label.Label == labelText.Trim())
                    return label;
            }

            return null;
        }

        public bool IsLabel(string line)
        {
            return line.EndsWith(":");
        }

        private bool IsEmptyLine(string line)
        {
            return string.IsNullOrWhiteSpace(line);
        }

        private bool IsCommentLine(string line)
        {
            return line.StartsWith(";");
        }
    }
}
