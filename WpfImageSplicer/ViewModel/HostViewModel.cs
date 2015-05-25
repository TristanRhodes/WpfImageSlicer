using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using WpfImageSplicer.Collections;
using WpfImageSplicer.Components;

namespace WpfImageSplicer.ViewModel
{
    public class HostViewModel : ViewModelBase
    {
        // Components
        private ILogger _logger;
        private IExceptionHandler _exceptionHandler;
        private IDialogService _dialogService;
        private IImageProcessor _imageProcessor;
        private IPixelMapBuilder _mapBuilder;
        
        // Property Backers
        private bool _processing;
        private BitmapImage _image;
        private string _imagePath;
        private ObservableCollection<PointCollection> _shapes = new ObservableCollection<PointCollection>();


        public HostViewModel(ILogger logger,
                                IExceptionHandler exceptionHandler,
                                IDialogService dialogService,
                                IImageProcessor imageProcessor,
                                IPixelMapBuilder mapBuilder)
        {
            // DI Setup
            _logger = logger;
            _exceptionHandler = exceptionHandler;
            _dialogService = dialogService;
            _imageProcessor = imageProcessor;
            _mapBuilder = mapBuilder;

            // Commands
            ProcessImageCommand = new RelayCommand(ExecuteProcessImage, CanExecuteProcessImage);
            BrowseForImageCommand = new RelayCommand(ExecuteBrowseForImage, CanExecuteBrowseForImage);
            ClearCommand = new RelayCommand(ExecuteClear, CanExecuteClear);
            ExportXamlCommand = new RelayCommand(ExecuteExportXaml, CanExecuteExportXaml);

            // Load default image
            Image = new BitmapImage(new Uri("pack://application:,,,/Resources/UKCounties.png"));
        }


        public bool Processing
        {
            get { return _processing; }

            set 
            {
                if (_processing == value)
                    return;

                _processing = value;
                RaisePropertyChanged(() => Processing);
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                if (_image == value)
                    return;

                _image = value;
                RaisePropertyChanged(() => Processing);
                RaisePropertyChanged(() => Width);
                RaisePropertyChanged(() => Height);
            }
        }

        public double Width 
        {
            get { return _image.PixelWidth; } 
        }

        public double Height 
        {
            get { return _image.PixelHeight; }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set 
            {
                if (_imagePath == value)
                    return;

                _imagePath = value;
                RaisePropertyChanged(() => ImagePath);
            }
        }

        public IEnumerable<PointCollection> Shapes 
        {
            get { return _shapes; }
        }


        public ICommand ProcessImageCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ICommand BrowseForImageCommand { get; private set; }

        public ICommand ExportXamlCommand { get; private set; }


        public bool CanExecuteProcessImage()
        {
            return !Processing;
        }

        public void ExecuteProcessImage()
        {
            Processing = true;

            // Load the map from the UI
            var pixels = _mapBuilder
                .GetPixels(Image);

            var task = _imageProcessor
                .ProcessImage(pixels)
                .ContinueWith(ProcessImageComplete, TaskScheduler.FromCurrentSynchronizationContext());
        }
        

        public bool CanExecuteBrowseForImage()
        {
            return !Processing;
        }

        public void ExecuteBrowseForImage()
        {
            var filename = _dialogService.SelectImage();
            if (filename == null)
                return;

            ImagePath = filename;
            Image = new BitmapImage(new Uri(filename));
        }

        
        public bool CanExecuteClear()
        {
            return !Processing;
        }

        public void ExecuteClear()
        {
            _shapes.Clear();
        }


        public bool CanExecuteExportXaml()
        {
            return this._shapes.Count > 0;
        }

        public void ExecuteExportXaml()
        {
            // Generate Xaml
            var xaml = GenerateXaml();

            // Package into message
            var msg = new XamlExportMessage();
            msg.Xaml = xaml;

            // Raise Message
            MessengerInstance.Send(msg);
        }


        private string GenerateXaml()
        {
            //HACK: Quick export implementation. To refactor out!!!!
            var converter = new WpfImageSplicer.Converters.PathConverter();

            // Setup Style
            var style = new System.Windows.Style();
            style.TargetType = typeof(System.Windows.Shapes.Path);

            style.Setters.Add(
                new System.Windows.Setter(System.Windows.Shapes.Path.StrokeThicknessProperty, 
                    1.0));

            style.Setters.Add(
                new System.Windows.Setter(System.Windows.Shapes.Path.StrokeProperty, 
                new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black)));

            style.Setters.Add(
                new System.Windows.Setter(System.Windows.Shapes.Path.FillProperty, 
                new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent)));

            var root = new System.Windows.Controls.Canvas();
            root.Width = Width;
            root.Height = Height;

            //TODO: Find a way to export the style as a resource, rather than embedded in the XAML.
            //root.Resources.Add("PathStyle", style);

            foreach(var shape in _shapes)
            {
                var path = new System.Windows.Shapes.Path();
                path.Data = (System.Windows.Media.PathGeometry)converter.Convert(shape, null, null, null);
                path.Style = style;
                //var binding = new System.Windows.Data.Binding("PathStyle");
                //path.SetBinding(System.Windows.Shapes.Path.StyleProperty, binding);
                root.Children.Add(path);
            }

            // TODO: Implement Proper XML Formatting.
            return System.Windows.Markup.XamlWriter.Save(root);
        }

        private void ProcessImageComplete(Task<List<PointCollection>> task)
        {
            Processing = false;

            if (task.Exception != null)
            {
                _exceptionHandler.HandleException(task.Exception);
                return;
            }

            _shapes = new ObservableCollection<PointCollection>(task.Result);
            RaisePropertyChanged(() => Shapes);
        }
    }
}
