using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageSplicer.Components
{
    /// <summary>
    /// Interface for handling top level exceptions. This should route
    /// to some kind of UI notification.
    /// </summary>
    public interface IExceptionHandler
    {
        void HandleException(Exception ex);
    }

    /// <summary>
    /// Default handler, bubbles up all exception.
    /// </summary>
    public class DefaultExceptionHandler : IExceptionHandler
    {
        public void HandleException(Exception ex)
        {
            throw ex;
        }
    }
}
