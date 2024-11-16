using System.Windows;
using System.Windows.Controls;

namespace TatehamaInterlockinglConsole.Handlers
{
    /// <summary>
    /// Labelクリックイベントハンドラー
    /// </summary>
    public class LabelHandler
    {
        public void AttachLabelClick(Label label, string clickEventName)
        {
            label.MouseDown += (s, e) =>
            {
                switch(clickEventName)
                {
                    default:
                        MessageBox.Show($"Click event {clickEventName}");
                    break;
                }
            };
        }
    }
}