using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageSplicer
{
    public struct CellState
    {
        public bool Open;
        public bool Explored;


        public CellState(bool open, bool explored)
        {
            this.Open = open;
            this.Explored = explored;
        }


        public override string ToString()
        {
            return string.Format("O: {0}, E: {1}", Open, Explored);
        }


        internal static CellState AsOpen()
        {
            return new CellState(true, false);
        }

        internal static CellState AsClosed()
        {
            return new CellState(false, false);
        }
    }
}
