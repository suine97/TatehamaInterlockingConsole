using System.Windows.Controls;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Handlers
{
    /// <summary>
    /// Labelクリックイベントハンドラー
    /// </summary>
    public class LabelHandler
    {
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
