using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileGameLib.Engine;
using TileGameLib.GameElements;

namespace PTM.Engine
{
    public class TileGameEngine : GameEngine
    {
        public TileGameEngine() : base("PTM - Programmable Tile Machine", 32, 24, 3, 100)
        {
            ObjectMap map = new ObjectMap("main", 32, 24, 1);
            AddMap(map, new BaseController());
            EnterMap(map);
        }
    }
}
