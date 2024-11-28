namespace TatehamaInterlockingConsole.ViewModels
{
    public class WindowViewModel : BaseViewModel
    {
        private double _windowWidth = 1024;
        private double _windowHeight = 768;
        private double _drawingWidth = 1024;
        private double _drawingHeight = 768;

        public double WindowWidth
        {
            get => _windowWidth;
            set => SetProperty(ref _windowWidth, value);
        }

        public double WindowHeight
        {
            get => _windowHeight;
            set => SetProperty(ref _windowHeight, value);
        }

        public double DrawingWidth
        {
            get => _drawingWidth;
            set => SetProperty(ref _drawingWidth, value);
        }

        public double DrawingHeight
        {
            get => _drawingHeight;
            set => SetProperty(ref _drawingHeight, value);
        }
    }
}
