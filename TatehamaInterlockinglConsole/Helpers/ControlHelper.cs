using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Helpers
{
    /// <summary>
    /// 汎用コントロール操作ヘルパークラス
    /// </summary>
    public static class ControlHelper
    {
        /// <summary>
        /// 親子関係を考慮した座標設定処理
        /// </summary>
        /// <param name="element"></param>
        /// <param name="setting"></param>
        public static void SetPosition(UIElement element, UIControlSetting setting, List<UIControlSetting> allSettings)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (!string.IsNullOrEmpty(setting.ParentName))
                {
                    var parentControl = FindControlByName(allSettings, setting.ParentName);
                    if (parentControl != null)
                    {
                        Canvas.SetLeft(element, parentControl.X + setting.RelativeX);
                        Canvas.SetTop(element, parentControl.Y + setting.RelativeY);
                    }
                    else
                    {
                        Canvas.SetLeft(element, setting.X);
                        Canvas.SetTop(element, setting.Y);
                    }
                }
                else
                {
                    Canvas.SetLeft(element, setting.X);
                    Canvas.SetTop(element, setting.Y);
                }
            });
        }

        /// <summary>
        /// コントロールリストから指定名称のコントロールを検索
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static UIControlSetting FindControlByName(List<UIControlSetting> allSettings, string name)
        {
            return allSettings.FirstOrDefault(control => control.UniqueName == name);
        }
    }
}
