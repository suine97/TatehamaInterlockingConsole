using System;
using System.IO;
using Jint;
using Jint.Native;

namespace TatehamaInterlockingConsole.JavaScript
{
    /// <summary>
    /// JavaScript関数実行クラス
    /// </summary>
    public class JavaScriptExecutor
    {
        private readonly Engine _engine;

        /// <summary>
        /// コンストラクタ: JavaScriptExecutorを初期化し、指定されたファイルをロード
        /// </summary>
        /// <param name="filePath">読み込むJavaScriptファイルのパス</param>
        public JavaScriptExecutor(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("ファイルパスが空です。", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("指定されたファイルが見つかりません。", filePath);

            // Jintエンジンを初期化
            _engine = new Engine();

            // JavaScriptファイルを読み込み、エンジンにロード
            string jsCode = File.ReadAllText(filePath);
            _engine.Execute(jsCode);
        }

        /// <summary>
        /// 指定したJavaScript関数を呼び出し
        /// </summary>
        /// <param name="functionName">呼び出すJavaScript関数名</param>
        /// <param name="args">JavaScript関数への引数</param>
        /// <returns>JavaScript関数の戻り値</returns>
        public object InvokeFunction(string functionName, params object[] args)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ArgumentException("関数名が空です。", nameof(functionName));

            // 関数が存在するかチェック
            JsValue function = _engine.GetValue(functionName);
            if (function.IsUndefined() || !function.IsObject())
            {
                throw new InvalidOperationException($"JavaScript関数 '{functionName}' は定義されていません。");
            }

            // 関数を呼び出す
            return _engine.Invoke(functionName, args).ToObject();
        }
    }
}
