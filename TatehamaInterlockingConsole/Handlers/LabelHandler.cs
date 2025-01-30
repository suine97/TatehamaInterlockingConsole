using System.Windows.Controls;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Handlers
{
    /// <summary>
    /// Labelクリックイベントハンドラー
    /// </summary>
    public class LabelHandler
    {
        private readonly ServerCommunication _serverCommunication;
        public static LabelHandler Instance { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serverCommunication"></param>
        public LabelHandler(ServerCommunication serverCommunication)
        {
            _serverCommunication = serverCommunication;
            Instance = this;
        }

        public void AttachLabelClick(Label label, UIControlSetting setting)
        {
            label.MouseLeftButtonDown += (s, e) =>
            {
                switch (setting.ClickEventName)
                {
                    default:
                        break;
                }
            };
            label.MouseRightButtonDown += (s, e) =>
            {
                switch (setting.ClickEventName)
                {
                    default:
                        break;
                }
            };
        }
    }
}
