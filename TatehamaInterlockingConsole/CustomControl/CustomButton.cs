using System.Windows;
using System.Windows.Controls;

namespace TatehamaInterlockingConsole.CustomControl
{
    public class CustomButton : Button
    {
        static CustomButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomButton), new FrameworkPropertyMetadata(typeof(CustomButton)));
        }
    }
}
