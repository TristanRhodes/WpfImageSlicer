using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using WpfImageSplicer.Components;
using WpfImageSplicer.View;
using WpfImageSplicer.ViewModel;


namespace WpfImageSplicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Window _xamlExportWindow = new XamlExportWindow();

        public MainWindow()
        {
            InitializeComponent();

            // Event Subscription
            Messenger.Default.Register<XamlExportMessage>(this, HandleExport);
        }

        private void HandleExport(XamlExportMessage obj)
        {
            _xamlExportWindow.Show();
            _xamlExportWindow.Activate();
        }
    }
}
