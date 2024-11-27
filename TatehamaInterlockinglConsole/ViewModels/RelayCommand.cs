using System;
using System.Windows.Input;

namespace TatehamaInterlockinglConsole.ViewModels
{
    /// <summary>
    /// パラメータなしでコマンドを実行するための汎用的なICommand実装
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute; // コマンド実行時に呼び出されるアクション
        private readonly Func<bool> _canExecute; // コマンドが実行可能かどうかを判定する関数

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute">コマンド実行時に実行されるアクション</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを判定する関数 (オプション)</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが現在実行可能かどうかを判定
        /// </summary>
        /// <param name="parameter">コマンドに渡されるパラメータ</param>
        /// <returns>コマンドが実行可能であればtrue、それ以外はfalse</returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="parameter">コマンドに渡されるパラメータ</param>
        public void Execute(object parameter) => _execute();

        /// <summary>
        /// コマンドの実行可能状態が変更されたときに発生
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    /// <summary>
    /// パラメータを伴うコマンドを実行するための汎用的なICommand実装
    /// </summary>
    /// <typeparam name="T">コマンドに渡されるパラメータの型</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute; // コマンド実行時に呼び出されるアクション
        private readonly Func<T, bool> _canExecute; // コマンドが実行可能かどうかを判定する関数

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute">コマンド実行時に実行されるアクション</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを判定する関数 (オプション)</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが現在実行可能かどうかを判定
        /// </summary>
        /// <param name="parameter">コマンドに渡されるパラメータ</param>
        /// <returns>コマンドが実行可能であればtrue、それ以外はfalse</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || (parameter is T t && _canExecute(t));
        }

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="parameter">コマンドに渡されるパラメータ</param>
        public void Execute(object parameter)
        {
            if (parameter is T t)
            {
                _execute(t);
            }
        }

        /// <summary>
        /// コマンドの実行可能状態が変更されたときに発生
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
