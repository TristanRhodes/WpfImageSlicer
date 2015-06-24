/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:WpfImageSplicer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using WpfImageSplicer.Components;

namespace WpfImageSplicer.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Needed to prevent design time errors.
                SimpleIoc.Default.Reset();
            }
            else
            {
                // Runtime view services and models
            }

            // Components and services.
            SimpleIoc.Default.Register<IDialogService, DefaultDialogService>();
            SimpleIoc.Default.Register<IExceptionHandler, DefaultExceptionHandler>();
            SimpleIoc.Default.Register<ILogger, TraceLogger>();
            SimpleIoc.Default.Register<IImageProcessor, ImageProcessor>();
            SimpleIoc.Default.Register<IPixelMapBuilder, PixelMapBuilder>();
            SimpleIoc.Default.Register<IXamlGenerator, DefaultXamlGenerator>();
            SimpleIoc.Default.Register<IExplorationMapBuilder, ExplorationMapBuilder>();


            // View Models
            SimpleIoc.Default.Register<HostViewModel>();
            SimpleIoc.Default.Register<XamlExportViewModel>();
            SimpleIoc.Default.Register<ColorPickerViewModel>();
        }

        public HostViewModel Host
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HostViewModel>();
            }
        }

        public XamlExportViewModel XamlExport
        {
            get
            {
                return ServiceLocator.Current.GetInstance<XamlExportViewModel>();
            }
        }

        public ColorPickerViewModel ColorPicker
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ColorPickerViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO: Clear the ViewModels
        }
    }
}