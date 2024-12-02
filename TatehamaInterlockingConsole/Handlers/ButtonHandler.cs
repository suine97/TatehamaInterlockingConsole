using System.Windows.Controls;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Handlers
{
    /// <summary>
    /// Buttonクリックイベントハンドラー
    /// </summary>
    public class ButtonHandler
    {
        public void AttachButtonClick(Button button, string clickEventName)
        {
            button.Click += (s, e) =>
            {
                switch (clickEventName)
                {
                    case "W11_江ノ原検車区":
                        WindowAction.ShowStationWindow("W11_江ノ原検車区表示盤");
                        WindowAction.ShowStationWindow("W11_江ノ原検車区操作盤");
                        break;
                    default:
                        WindowAction.ShowStationWindow(clickEventName);
                        break;
                }
            };
        }
    }
}