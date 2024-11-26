using System.Windows;
using System.Collections.ObjectModel;
using TatehamaInterlockinglConsole.Factories;
using System.Collections.Generic;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.Services
{
    public static class CreateUIControl
    {
        public static ObservableCollection<UIElement> CreateUIControlAsUIElement(List<UIControlSetting> allSettings)
        {
            var elements = new ObservableCollection<UIElement>();

            // コントロール作成
            foreach (var setting in allSettings)
            {
                var control = ControlFactory.CreateControl(setting, allSettings);
                if (control != null)
                {
                    elements.Add(control);
                }
            }
            return elements;
        }
    }
}
