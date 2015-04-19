using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfImageSplicer.Components;


namespace WpfImageSplicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Brush _borderBrush = new SolidColorBrush(Colors.Green);

        public MainWindow()
        {
            _borderBrush.Opacity = 0.75;
            InitializeComponent();
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            ProcessButton.IsEnabled = false;

            // Load the map on the UI 
            var source = (BitmapImage)LoadedImage.Source;
            var pixels = LoadMap(source);

            var task = Task
                .Run<List<List<Point>>>(() => ProcessImage(pixels))
                .ContinueWith(ProcessImageComplete, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            var children = Canvas.Children
                            .OfType<Path>()
                            .ToList();

            children
                .ForEach(c => Canvas.Children.Remove(c));
        }

        private List<List<Point>> ProcessImage(PixelColor[,] pixels)
        {
            var map = ExploreMap(pixels);

            var shapeDetector = new ShapeDetector(map);
            var edgePlotter = new EdgePlotter(new TraceLogger());

            List<List<Point>> edgeList = new List<List<Point>>();

            var shapeMap = shapeDetector.CreateShapeMap();

            while (shapeDetector.GenerateShape(shapeMap))
            {
                var edge = edgePlotter.CalculateEdge(shapeMap);

                if (edge.Count > 2)
                    edgeList.Add(edge);
            }

            return edgeList;
        }

        private void ProcessImageComplete(Task<List<List<Point>>> edgeList)
        {
            ProcessButton.IsEnabled = true;

            if (edgeList.Exception != null)
            {
                ShowException(edgeList.Exception);
                return;
            }

            foreach(var edge in edgeList.Result)
            {
                var path = CreatePath(edge);
                Canvas.Children.Add(path);
            }
        }

        private Path CreatePath(List<Point> edge)
        {
            var start = new System.Windows.Point(edge[0].X, edge[0].Y);;

            List<LineSegment> segments = new List<LineSegment>();
            for (int i = 1; i < edge.Count; i++)
            {
                var edgePoint = edge[i];
                var wpfPoint = new System.Windows.Point(edgePoint.X, edgePoint.Y);
                segments.Add(new LineSegment(wpfPoint, true));
            }

            PathFigure figure = new PathFigure(start, segments, true); //true if closed
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);


            var path = new Path();
            path.StrokeThickness = 4;
            path.Stroke = _borderBrush;
            path.Data = geometry;
            return path;
        }

        private void ShowException(Exception ex)
        {
            System.Diagnostics.Debugger.Break();
            throw ex;
        }

        public static PixelColor[,] LoadMap(BitmapSource source)
        {
            var pixelMapBuilder = new PixelMapBuilder();
            var pixels = pixelMapBuilder.GetPixels(source);
            return pixels;
        }

        public static MapState[,] ExploreMap(PixelColor[,] pixels)
        {
            var mapBuilder = new ExplorationMapBuilder(20);
            var map = mapBuilder.GetExplorationMap(pixels);
            return map;
        }
    }
}
