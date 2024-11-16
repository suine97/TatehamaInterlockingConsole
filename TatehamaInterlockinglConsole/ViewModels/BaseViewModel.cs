using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TatehamaInterlockinglConsole.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // INotifyPropertyChangedのイベント
        public event PropertyChangedEventHandler PropertyChanged;

        // プロパティ変更時に呼ばれるメソッド
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // プロパティの変更通知を簡潔に行うメソッド
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
