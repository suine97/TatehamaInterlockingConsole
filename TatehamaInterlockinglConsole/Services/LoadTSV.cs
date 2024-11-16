using System.Windows;
using System.Collections.ObjectModel;
using TatehamaInterlockinglConsole.Factories;

namespace TatehamaInterlockinglConsole.Services
{
    public static class LoadTSV
    {
        public static ObservableCollection<UIElement> LoadUIFromTSV(string filePath)
        {
            var elements = new ObservableCollection<UIElement>();
            var settings = UIControlSettingLoader.LoadSettings(filePath);
            foreach (var setting in settings)
            {
                var control = ControlFactory.CreateControl(setting, settings);
                if (control != null)
                {
                    elements.Add(control);
                }
            }
            return elements;
        }
    }
}
