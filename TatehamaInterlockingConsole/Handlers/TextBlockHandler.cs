using System.Windows.Controls;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Handlers
{
    /// <summary>
    /// TextBlockクリックイベントハンドラー
    /// </summary>
    public class TextBlockHandler
    {
        private readonly ServerCommunication _serverCommunication;
        public static TextBlockHandler Instance { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serverCommunication"></param>
        public TextBlockHandler(ServerCommunication serverCommunication)
        {
            _serverCommunication = serverCommunication;
            Instance = this;
        }

        public void AttachTextBlockClick(Grid grid, string clickEventName)
        {
            grid.MouseDown += (s, e) =>
            {
                switch(clickEventName)
                {
                    default:
                        break;
                }
            };
        }
    }
}