using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace WpfImageSplicer.ViewModel
{
    public class ColorPickerViewModel :  ViewModelBase
    {
        private bool _sampleMode = false;

        private int _tolerance;
        private Color _color;


        public ColorPickerViewModel()
        {
            // Defaults
            _tolerance = 20;
            _color = Color.FromRgb(255, 255, 255);

            // Commands
            SampleCommand = new RelayCommand(ExecuteSample, CanExecuteSample);
            CancelSampleCommand = new RelayCommand(ExecuteCancelSample, CanExecuteCancelSample);
        }


        public ICommand SampleCommand { get; private set; }

        public ICommand CancelSampleCommand { get; private set; }


        public int Tolerance
        {
            get { return _tolerance; }
            set
            {
                if (_tolerance == value)
                    return;

                _tolerance = value;
                RaisePropertyChanged(() => Tolerance);
                OnColorSelectionChanged();
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value)
                    return;

                _color = value;
                RaisePropertyChanged(() => Color);
                OnColorSelectionChanged();
            }
        }


        private bool CanExecuteSample()
        {
            return !_sampleMode;
        }

        private void ExecuteSample()
        {
            _sampleMode = !_sampleMode;
            OnStartSample();
        }


        private bool CanExecuteCancelSample()
        {
            return _sampleMode;
        }

        private void ExecuteCancelSample()
        {
            _sampleMode = !_sampleMode;
            OnEndSample();
        }


        private void OnStartSample()
        {
            var msg = new BeginColorSampleMode(ColorPickCallback);
            this.MessengerInstance.Send(msg);
        }

        private void OnEndSample()
        {
            this.MessengerInstance.Send(new EndColorSampleMode());
        }

        private void OnColorSelectionChanged()
        {
            var msg = new ColorSelectionChangedEventArgs(_tolerance, _color);
            this.MessengerInstance.Send(msg);
        }


        private void ColorPickCallback(Color color)
        {
            Color = color;
            ExecuteCancelSample();
        }
    }

    /// <summary>
    /// This event is raised when the color picker enters sample mode.
    /// </summary>
    public class BeginColorSampleMode
    {
        public BeginColorSampleMode(Action<Color> selectColorCallback)
        {
            SelectColorCallback = selectColorCallback;
        }

        public Action<Color> SelectColorCallback { get; private set; }
    }

    /// <summary>
    /// This event is raised when the color picker exits sample mode.
    /// Used by the wider application to unlock functionality.
    /// </summary>
    public class EndColorSampleMode
    {
        // Nothing here
    }

    /// <summary>
    /// Event raised when a the color picker option is changed
    /// </summary>
    public class ColorSelectionChangedEventArgs
    {
        public ColorSelectionChangedEventArgs(int tolerance, Color color)
        {
            Tolerance = tolerance;
            Color = color;
        }

        public int Tolerance { get; private set; }

        public Color Color { get; private set; }
    }
}
