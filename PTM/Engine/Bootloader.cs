using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTM.Engine
{
    public class Bootloader
    {
        private const string ConfigFile = "ptm.ini";

        public void Start()
        {
            string programFile = null;

            try
            {
                programFile = TryLoadProgramFile();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show($"Program file not found: {programFile}", "PTM boot error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PTM boot error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (programFile != null)
            {
                Machine machine = new Machine(new Program(programFile));
                machine.Run();
            }
            else
            {
                MessageBox.Show("No program file specified", "PTM boot error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string TryLoadProgramFile()
        {
            string file = null;

            if (File.Exists(ConfigFile))
            {
                string[] configs = File.ReadAllLines(ConfigFile);
                if (configs.Length > 0)
                    file = configs[0];
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (dialog.ShowDialog() == DialogResult.OK)
                    file = dialog.FileName;
            }

            return file;
        }
    }
}
