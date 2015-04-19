using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageSplicer.Components
{
    public interface ILogger
    {
        void Trace(string message);
    }

    public class TraceLogger : ILogger
    {
        public void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }
    }
}
