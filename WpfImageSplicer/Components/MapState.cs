using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageSplicer.Components
{
    public struct MapState
    {
        public bool Open;
        public bool Explored;


        private MapState(bool open, bool explored)
        {
            this.Open = open;
            this.Explored = explored;
        }


        public MapState AsExplored()
        {
            return new MapState(Open, true);
        }


        public override string ToString()
        {
            return string.Format("O: {0}, E: {1}", Open, Explored);
        }


        internal static MapState AsOpen()
        {
            return new MapState(true, false);
        }

        internal static MapState AsClosed()
        {
            return new MapState(false, false);
        }
    }
}
