using System;
using System.Text.RegularExpressions;

namespace TatehamaInterlockingConsole.Factories
{
    public static class RetsubanFactory
    {
        /// <summary>
        /// Retsubanコントロール作成処理
        /// </summary>
        /// <param name="retsuban"></param>
        public static void CreateRetsubanControl(string retsuban)
        {
            // 正規表現パターンの定義
            var pattern = @"([回試臨]?)([0-9]{0,4})(A|B|C|K|X|AX|BX|CX|KX)?$";
            var match = Regex.Match(retsuban, pattern);

            if (!match.Success) return;

            // 先頭文字を描画
            DrawHeadSymbol(match.Groups[1].Value);

            // 数字部分を描画
            DrawDigits(match.Groups[2].Value);

            // 接尾文字を描画
            DrawTailSymbol(match.Groups[3].Value);
        }

        // 先頭文字の描画処理
        private static void DrawHeadSymbol(string head)
        {
            switch (head)
            {
                case "回":
                    Draw("回");
                    break;
                case "試":
                    Draw("試");
                    break;
                case "臨":
                    Draw("臨");
                    break;
                default:
                    Draw("無");
                    break;
            }
        }

        // 数字部分の描画処理
        private static void DrawDigits(string digits)
        {
            string paddedDigits = digits.PadLeft(4, ' ');
            for (int i = 0; i < paddedDigits.Length; i++)
            {
                Draw(paddedDigits[i].ToString());
            }
        }

        // 接尾文字の描画処理
        private static void DrawTailSymbol(string tail)
        {
            switch (tail)
            {
                case "A":
                    Draw("A");
                    break;
                case "B":
                    Draw("B");
                    break;
                case "C":
                    Draw("C");
                    break;
                case "K":
                    Draw("K");
                    break;
                case "X":
                    Draw("X");
                    break;
                case "AX":
                    Draw("AX");
                    break;
                case "BX":
                    Draw("BX");
                    break;
                case "CX":
                    Draw("CX");
                    break;
                case "KX":
                    Draw("KX");
                    break;
                default:
                    Draw("無");
                    break;
            }
        }

        // 描画処理の共通メソッド（仮定的な描画メソッド）
        private static void Draw(string symbol)
        {
            // 実際の描画処理をここに実装します
            Console.WriteLine(symbol); // デバッグ用出力
        }
    }
}
