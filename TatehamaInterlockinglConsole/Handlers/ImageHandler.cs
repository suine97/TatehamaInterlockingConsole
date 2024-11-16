using System.Windows;
using System.Windows.Controls;

namespace TatehamaInterlockinglConsole.Handlers
{
    /// <summary>
    /// Imageクリックイベントハンドラー
    /// </summary>
    public class ImageHandler
    {
        public void AttachImageClick(Image image, string clickEventName)
        {
            image.MouseDown += (s, e) =>
            {
                switch (clickEventName)
                {
                    default:
                        MessageBox.Show($"Click event {clickEventName}");
                        break;
                }
            };
        }
    }
}
