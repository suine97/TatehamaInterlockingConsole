using System.Windows;
using System.Windows.Controls;

namespace TatehamaInterlockinglConsole.CustomControl
{
    public class CustomButton : Button
    {
        static CustomButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomButton), new FrameworkPropertyMetadata(typeof(CustomButton)));
        }
    }
}
