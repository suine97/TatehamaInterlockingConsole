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
                    case "TH66S_Enohara-Inspection":
                        WindowAction.ShowStationWindow("TH66S_Enohara-Inspection-Indicater");
                        WindowAction.ShowStationWindow("TH66S_Enohara-Inspection-Switchboard");
                        break;
                    default:
                        WindowAction.ShowStationWindow(clickEventName);
                        break;
                }
            };
        }
    }
}