using System.Windows.Controls;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Handlers
{
    /// <summary>
    /// Buttonクリックイベントハンドラー
    /// </summary>
    public class ButtonHandler
    {
        private readonly ServerCommunication _serverCommunication;
        public static ButtonHandler Instance { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serverCommunication"></param>
        public ButtonHandler(ServerCommunication serverCommunication)
        {
            _serverCommunication = serverCommunication;
            Instance = this;
        }

        public void AttachButtonClick(Button button, string clickEventName)
        {
            button.Click += (s, e) =>
            {
                // サーバー接続状態の場合のみ処理を実行
                //if (DataManager.Instance.ServerConnected)
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
                }
            };
        }
    }
}