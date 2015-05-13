using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfImageSplicer.Components;
using WpfImageSplicer.ViewModel;


namespace WpfImageSplicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage DefaultImage
        {
            get { return (BitmapImage)this.Resources["MapImage"]; }
        }

        private HostViewModel ViewModel
        {
            get { return (HostViewModel)this.DataContext; }
            set { this.DataContext = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new HostViewModel();
            ViewModel.Image = DefaultImage;
        }
    }
}
