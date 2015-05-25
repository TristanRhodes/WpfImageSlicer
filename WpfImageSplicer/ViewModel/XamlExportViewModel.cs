using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WpfImageSplicer.ViewModel
{
    public class XamlExportViewModel : ViewModelBase
    {
        private string _xaml;


        public XamlExportViewModel()
        {
            MessengerInstance.Register<XamlExportMessage>(this, HandleExport);
        }


        public string Xaml
        {
            get { return _xaml; }

            private set
            {
                if (_xaml == value)
                    return;

                _xaml = value;
                RaisePropertyChanged(() => Xaml);
            }
        }


        private void HandleExport(XamlExportMessage msg)
        {
            Xaml = msg.Xaml;
        }
    }

    public class XamlExportMessage
    {
        /// <summary>
        /// Xaml body to be exported
        /// </summary>
        public string Xaml { get; set; }
    }
}
