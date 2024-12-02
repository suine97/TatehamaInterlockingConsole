using System.Windows;
using System.Windows.Controls;

namespace TatehamaInterlockingConsole.Handlers
{
    /// <summary>
    /// TextBlockクリックイベントハンドラー
    /// </summary>
    public class TextBlockHandler
    {
        public void AttachTextBlockClick(Grid grid, string clickEventName)
        {
            grid.MouseDown += (s, e) =>
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