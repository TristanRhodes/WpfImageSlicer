using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WpfImageSplicer.Components
{
    /// <summary>
    /// Basic Dialog service to hide popups.
    /// </summary>
    public interface IDialogService
    {
        string SelectImage();
    }

    public class DefaultDialogService : IDialogService
    {
        public string SelectImage()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter =
                "Image Files(*.BMP;*.JPG;*.GIF;*.JPEG;*.PNG)|*.BMP;*.JPG;*.GIF;*.JPEG;*.PNG|All files (*.*)|*.*"
            };

            var result = dlg.ShowDialog();

            if (result == null)
                return null;

            if (result.Value)
                return dlg.FileName;

            return null;
        }
    }
}
