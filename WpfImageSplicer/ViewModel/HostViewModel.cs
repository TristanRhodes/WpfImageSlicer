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
        private IXamlGenerator _xamlGenerator;

        // Property Backers
        private bool _processing;
        private BitmapImage _image;
        private string _imagePath;
        private ObservableCollection<PointCollection> _shapes = new ObservableCollection<PointCollection>();
        

        public HostViewModel(ILogger logger,
                                IExceptionHandler exceptionHandler,
                                IDialogService dialogService,
                                IImageProcessor imageProcessor,
                                IPixelMapBuilder mapBuilder,
                                IXamlGenerator xamlGenerator)
        {

            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            // DI Setup
            _logger = logger;
            _exceptionHandler = exceptionHandler;
            _dialogService = dialogService;
            _imageProcessor = imageProcessor;
            _mapBuilder = mapBuilder;
            _xamlGenerator = xamlGenerator;

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

                // Lots of properties are impacted by a change in image, 
                // but they are derived values, so have no direct setter.
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

            // Kick off a task to process the image, and handle the completed callback on the UI thread.
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
            var xaml = _xamlGenerator.GenerateXaml(Width, Height, _shapes);

            // Package into message
            var msg = new XamlExportMessage();
            msg.Xaml = xaml.Result;

            // Raise Message
            MessengerInstance.Send(msg);
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

            //NOTE: This is not ideal. Should find better solution. 
            // Without this, buttons do not re-enable after processing is completed, and remain grayed out.
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
