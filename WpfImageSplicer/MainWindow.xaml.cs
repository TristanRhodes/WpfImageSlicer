using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private Window _xamlExportWindow = new XamlExportWindow();

        /// <summary>
        /// Callback for color picker state. If this is null, ignore clicks,
        /// otherwise supply the color of the current images pixel to the control.
        /// </summary>
        private Action<Color> _colorPickerCallback;


        public MainWindow()
        {
            InitializeComponent();

            // Event Subscription
            Messenger.Default.Register<XamlExportMessage>(this, HandleExport);
            
            // Events for color picker. Need to route click events through current UI layer
            // and skim the color of the pixel.
            Messenger.Default.Register<BeginColorSampleMode>(this, HandleBeginColorSample);
            Messenger.Default.Register<EndColorSampleMode>(this, HandleEndColorSample);
        }


        private void HandleBeginColorSample(BeginColorSampleMode e)
        {
            _colorPickerCallback = e.SelectColorCallback;
        }

        private void HandleEndColorSample(EndColorSampleMode e)
        {
            _colorPickerCallback = null;
        }


        private void HandleExport(XamlExportMessage e)
        {
            _xamlExportWindow.Show();
            _xamlExportWindow.Activate();
        }

        protected override void OnClosed(EventArgs e)
        {
            App.Current.Shutdown(0);
            ViewModelLocator.Cleanup();

            base.OnClosed(e);
        }



        // TODO: Switch this over to a Behavior. Get the code out of the UI.
        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore if we don't have a callback
            if (_colorPickerCallback == null)
                return;

            var color = SampleColor();

            if (color.HasValue)
                _colorPickerCallback(color.Value);
        }

        private Color? SampleColor()
        {
            // Retrieve the coordinate of the mouse position in relation to the supplied image.
            var point = Mouse.GetPosition(LoadedImage);

            // Use RenderTargetBitmap to get the visual, in case the image has been transformed.
            var renderTargetBitmap = new RenderTargetBitmap((int)LoadedImage.ActualWidth,
                                                            (int)LoadedImage.ActualHeight,
                                                            96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(LoadedImage);

            // Make sure that the point is within the dimensions of the image.
            if ((point.X <= renderTargetBitmap.PixelWidth) && (point.Y <= renderTargetBitmap.PixelHeight))
            {
                // Create a cropped image at the supplied point coordinates.
                var croppedBitmap = new CroppedBitmap(renderTargetBitmap,
                                                      new Int32Rect((int)point.X, (int)point.Y, 1, 1));

                // Copy the sampled pixel to a byte array.
                var pixels = new byte[4];
                croppedBitmap.CopyPixels(pixels, 4, 0);

                // Assign the sampled color to a SolidColorBrush and return as conversion.
                return Color.FromArgb(255, pixels[2], pixels[1], pixels[0]);
            }

            return null;
        }
    }
}
