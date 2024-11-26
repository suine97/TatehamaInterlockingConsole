using System;
using System.IO;

namespace TatehamaInterlockinglConsole.Services
{
    public static class Data
    {
        /// <summary>
        /// 偶数かどうかを判定する
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsEven(int num)
        {
            return (num % 2 == 0);
        }

        /// <summary>
        /// 実行ファイルのフォルダ名を取得
        /// </summary>
        /// <returns></returns>
        public static string GetAppPath()
        {
            return Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// ファイル名から駅名を取得
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetStatinNameFromFilePath(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath).Split('_')[0]
                   + "_"
                   + Path.GetFileNameWithoutExtension(filePath).Split('_')[1];
        }
    }

    /// <summary>
    /// float型拡張クラス
    /// </summary>
    public static class FloatExtensions
    {
        public static bool IsZero(this float self)
        {
            return self.IsZero(float.Epsilon);
        }

        public static bool IsZero(this float self, float epsilon)
        {
            return Math.Abs(self) < epsilon;
        }
    }
}
