﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WpfImageSplicer.Collections;
using WpfImageSplicer.Components;

namespace WpfImageSplicer.ViewModel
{
    public class HostViewModel : MainViewModel
    {
        private bool _processing;
        private BitmapImage _image;
        private ObservableCollection<PointCollection> _shapes = new ObservableCollection<PointCollection>();
        private string _imagePath;


        public HostViewModel()
        {
            ProcessImageCommand = new RelayCommand(ExecuteProcessImage, CanExecuteProcessImage);
            BrowseForImageCommand = new RelayCommand(ExecuteBrowseForImage, CanExecuteBrowseForImage);
            ClearCommand = new RelayCommand(ExecuteClear, CanExecuteClear);
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
            get { return _image.Width; } 
        }

        public double Height 
        {
            get { return _image.Height; }
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


        public bool CanExecuteProcessImage()
        {
            return !Processing;
        }

        public void ExecuteProcessImage()
        {
            Processing = true;

            // Load the map on the UI 
            var pixels = LoadMap();

            var task = Task
                .Run<List<PointCollection>>(() => ProcessImage(pixels))
                .ContinueWith(ProcessImageComplete, TaskScheduler.FromCurrentSynchronizationContext());
        }
        

        public bool CanExecuteBrowseForImage()
        {
            return !Processing;
        }

        public void ExecuteBrowseForImage()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".png",
                Filter =
                    "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                // Open document 
                var filename = dlg.FileName;
                ImagePath = filename;
                Image = new BitmapImage(new Uri(filename));
            }
        }

        
        public bool CanExecuteClear()
        {
            return !Processing;
        }

        public void ExecuteClear()
        {
            _shapes.Clear();
        }


        private List<PointCollection> ProcessImage(PixelColor[,] pixels)
        {
            var map = ExploreMap(pixels);

            var shapeDetector = new ShapeDetector(map);
            var edgePlotter = new EdgePlotter(new TraceLogger());

            var edgeList = new List<PointCollection>();

            var shapeMap = shapeDetector.CreateShapeMap();

            while (shapeDetector.GenerateShape(shapeMap))
            {
                var edge = edgePlotter.CalculateEdge(shapeMap);

                if (edge.Count > 2)
                    edgeList.Add(edge);
            }

            return edgeList;
        }

        private void ProcessImageComplete(Task<List<PointCollection>> edgeList)
        {
            Processing = false;

            if (edgeList.Exception != null)
            {
                HandleException(edgeList.Exception);
                return;
            }

            foreach (var edge in edgeList.Result)
            {
                _shapes.Add(edge);
            }
        }

        public PixelColor[,] LoadMap()
        {
            var pixelMapBuilder = new PixelMapBuilder();
            var pixels = pixelMapBuilder.GetPixels(Image);
            return pixels;
        }

        public static MapState[,] ExploreMap(PixelColor[,] pixels)
        {
            var mapBuilder = new ExplorationMapBuilder(20);
            var map = mapBuilder.GetExplorationMap(pixels);
            return map;
        }


        private void HandleException(Exception ex)
        {
            //TODO: Exception Policy
            System.Diagnostics.Debugger.Break();
            throw ex;
        }
    }
}