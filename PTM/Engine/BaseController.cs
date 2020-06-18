using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TileGameLib.Engine;

namespace PTM.Engine
{
    public class BaseController : MapController
    {
        public override void OnEnter()
        {
            //Debugger.Log("OnStart");
            //Debugger.Show();

            /*
            Window.Size = new Size(500, 500);
            ObjectMap map = new ObjectMap("main", 20, 18, 1);
            map.SetHorizontalStringOfObjects("Hello World!", new ObjectPosition(0, 0, 0), 0, 1);
            AddMap(map, new BaseController());
            EnterMap(map);*/
        }

        public override void OnExecuteCycle()
        {
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
        }

        public override void OnKeyUp(KeyEventArgs e)
        {
        }
    }
}
