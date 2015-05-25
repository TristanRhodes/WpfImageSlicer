using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfImageSplicer.View
{
    /// <summary>
    /// Interaction logic for XamlExportWindow.xaml
    /// </summary>
    public partial class XamlExportWindow : Window
    {
        public XamlExportWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Hide and cancel the close event so the window can be reused.
            this.Hide();
            e.Cancel = true;

            base.OnClosing(e);
        }
    }
}
