using System.Windows;
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
                        MessageBox.Show($"Label Left Click event {setting.ClickEventName} | {setting.UniqueName}");
                        break;
                }
            };
            label.MouseRightButtonDown += (s, e) =>
            {
                switch (setting.ClickEventName)
                {
                    default:
                        MessageBox.Show($"Label Right Click event {setting.ClickEventName} | {setting.UniqueName}");
                        break;
                }
            };
        }
    }
}
