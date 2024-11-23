using System.Windows;
using System.Windows.Controls;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.Handlers
{
    /// <summary>
    /// Imageクリックイベントハンドラー
    /// </summary>
    public class ImageHandler
    {
        public void AttachImageClick(Image image, UIControlSetting setting)
        {
            image.MouseLeftButtonDown += (s, e) =>
            {
                switch (setting.ClickEventName)
                {
                    default:
                        MessageBox.Show($"Left Click event {setting.ClickEventName} | {setting.UniqueName}");
                        break;
                }
            };
            image.MouseRightButtonDown += (s, e) =>
            {
                switch (setting.ClickEventName)
                {
                    default:
                        MessageBox.Show($"Right Click event {setting.ClickEventName} | {setting.UniqueName}");
                        break;
                }
            };
        }
    }
}
