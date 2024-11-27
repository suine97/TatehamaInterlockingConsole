using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TatehamaInterlockinglConsole.ViewModels
{
    /// <summary>
    /// ViewModelクラスの基底クラスとしてプロパティ変更通知をサポート
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティが変更されたときに発生するイベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティ変更通知をトリガー
        /// </summary>
        /// <param name="propertyName">変更されたプロパティの名前。省略すると呼び出し元のプロパティ名が使用される</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティの値を設定し、必要に応じて変更通知をトリガー
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="field">プロパティに対応するフィールド</param>
        /// <param name="value">新しい値</param>
        /// <param name="propertyName">変更されたプロパティの名前。省略すると呼び出し元のプロパティ名が使用される</param>
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            // フィールドの値が変更された場合のみ更新と通知を実行
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
