using System;
using System.Collections.Generic;
using System.Linq;

namespace TatehamaInterlockingConsole.Services
{
    public static class TextParse
    {
        private static int loop = 0;

        /// <summary>
        /// てこの内容をパースする関数
        /// </summary>
        /// <param name="input">パースする文字列</param>
        /// <param name="depth">深さ</param>
        /// <param name="station">所属駅</param>
        /// <param name="kata">片鎖錠かどうか</param>
        /// <returns>パース結果のオブジェクトの配列</returns>
        public static List<ParseResult> TekoParse(string input, int depth = 0, int station = 0, bool kata = false)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new();
            }
            loop++;

            // 意味区切り括弧・所属駅括弧のうち速い方を見つける。
            int resultIndex = FindLastTargetIndex(input, new[] { "}", "]", "但" });
            if (resultIndex == 0)
            {
                // 意味区切り括弧
                var r = SplitStringAtTwoTargets(input, "{", "}");
                if (r.Position1 != -1)
                {
                    // 前中後すべてに再帰的実行
                    var b = TekoParse(r.Before, depth + 1, station);
                    var w = TekoParse(r.Between, depth + 1, station);
                    var a = TekoParse(r.After, depth + 1, station);
                    return b.Concat(w).Concat(a).ToList();
                }
            }
            else if (resultIndex == 1)
            {
                // 所属駅括弧
                var count = CountConsecutiveTargetsFromEnd(input, "]").Count;
                var r = SplitStringAtTwoTargets(input, new string('[', count), new string(']', count));
                if (r.Position1 != -1)
                {
                    // 前中後すべてに再帰的実行
                    var b = TekoParse(r.Before, depth + 1, station);
                    var w = TekoParse(r.Between, depth + 1, station + count);
                    var a = TekoParse(r.After, depth + 1, station);
                    return b.Concat(w).Concat(a).ToList();
                }
            }
            else if (resultIndex == 2)
            {
                // 但文節
                var r = SplitStringAtTarget(input, "但");
                var b = TekoParse(r.Before, depth + 1, station);
                var a = TekoParse(r.After, depth + 1, station);

                // 但の前のexecuteに後ろを入れる
                if (b[0].Type == "or")
                {
                    b[0].Execute.ForEach(e => e.Execute = a);
                }
                else
                {
                    b.ForEach(e => e.Execute = a);
                }
                return b;
            }
            else
            {
                // 意味区切り系括弧等がない状態まで分割した
                if (input.Contains("又は"))
                {
                    var array = ApplyFunctionToArray(input.Split("又は"), TekoParse, depth + 1, station);
                    return new List<ParseResult>
                    {
                        new() {
                            Type = "or",
                            Name = input,
                            Execute = array.SelectMany(x => x).ToList()
                        }
                    };
                }
                else if (input.Contains(" "))
                {
                    var array = ApplyFunctionToArray(input.Split(" "), TekoParse, depth + 1, station);
                    return array.SelectMany(x => x).ToList();
                }
                else if (input.Contains(">"))
                {
                    var r = SplitStringAtTwoTargets(input, "<", ">");
                    if (r.Position1 != -1)
                    {
                        var b = TekoParse(r.Before, depth + 1, station);
                        var w = TekoParse(r.Between, depth + 1, station, true);
                        var a = TekoParse(r.After, depth + 1, station);
                        return b.Concat(w).Concat(a).ToList();
                    }
                }
                else if (input.Contains(")("))
                {
                    var count = CountConsecutiveTargetsFromEnd(input, "(").Count;
                    var r = SplitStringAtTwoTargets(input, new string('(', count), new string(')', count));
                    if (r.Position1 != -1)
                    {
                        var b = TekoParse(r.Before, depth + 1, station);
                        var w = TekoParse(new string('(', count) + r.Between + new string(')', count), depth + 1, station);
                        var a = TekoParse(r.After, depth + 1, station);
                        return b.Concat(w).Concat(a).ToList();
                    }
                }
                else
                {
                    // てこ・回路単体
                    bool teihan = false;
                    bool sokatsu = false;
                    if (input.Contains("(("))
                    {
                        sokatsu = true;
                        input = input.Replace("((", "").Replace("))", "");
                    }
                    else if (input.Contains("("))
                    {
                        teihan = true;
                        input = input.Replace("(", "").Replace(")", "");
                    }

                    // タイプ確認
                    string type = "Null";
                    if (SaveData.TekoTypeObj.TryGetValue(input, out TekoType value))
                    {
                        type = value.Type;
                    }
                    else if (input.EndsWith("T"))
                    {
                        type = "Track";
                    }
                    else if (input.EndsWith("秒"))
                    {
                        type = "Timer";
                        input = input.Replace("秒", "");
                    }

                    return new List<ParseResult>
                    {
                        new() {
                            Type = type,
                            Station = station,
                            Name = input,
                            Teihan = teihan,
                            Execute = new List<ParseResult>(),
                            Sokatsu = sokatsu,
                            Kata = kata
                        }
                    };
                }
            }

            return new();
        }

        /// <summary>
        /// 進路区分鎖錠に対応するパース
        /// </summary>
        public static List<ParseResult> RouteLockParse(string input)
        {
            List<ParseResult> r = new();
            if (input.StartsWith('(') && input.EndsWith(')'))
            {
                input = input[1..^1];
            }
            input = input.Replace(") (", ")(");
            foreach (var e in input.Split(new[] { ")(" }, StringSplitOptions.None))
            {
                r.AddRange(TekoParse(e));
            }
            return r;
        }

        /// <summary>
        /// 配列のすべての要素に対して特定の関数を適用する関数
        /// </summary>
        public static List<List<ParseResult>> ApplyFunctionToArray(string[] array, Func<string, int, int, bool, List<ParseResult>> func, int depth, int station)
        {
            return array.Select(element => func(element, depth, station, false)).Where(element => element != null).ToList();
        }

        /// <summary>
        /// 元の文字列と対象文字列の配列を受け取り、対象文字列のうち最後から数えて最も早く出現する文字列のインデックスを返す関数
        /// </summary>
        public static int FindLastTargetIndex(string str, string[] targets)
        {
            int latestIndex = -1;
            int targetIndex = -1;

            for (int i = 0; i < targets.Length; i++)
            {
                int index = str.LastIndexOf(targets[i], StringComparison.Ordinal);
                if (index != -1 && (latestIndex == -1 || index > latestIndex))
                {
                    latestIndex = index;
                    targetIndex = i;
                }
            }

            return targetIndex;
        }

        /// <summary>
        /// 指定された文字列が最後に発見された箇所で、その文字列が何回連続で登場するかをカウントする関数
        /// </summary>
        public static (int Position, int Count) CountConsecutiveTargetsFromEnd(string str, string target)
        {
            int position = str.LastIndexOf(target, StringComparison.Ordinal);

            if (position == -1)
            {
                return (-1, 0);
            }

            int count = 0;
            int i = position;

            while (i >= 0 && str.Substring(i, target.Length) == target)
            {
                count++;
                i -= target.Length;
            }

            return (position, count);
        }

        /// <summary>
        /// 文字列内にある特定の文字列を最後から探し出し、その文字の位置を知り、文字列をその位置で分割する関数
        /// </summary>
        public static (string Before, string Target, string After, int Position) SplitStringAtTarget(string str, string target)
        {
            int position = str.LastIndexOf(target, StringComparison.Ordinal);

            if (position == -1)
            {
                return (str, null, null, -1);
            }

            string before = str.Substring(0, position);
            string after = str.Substring(position + target.Length);

            return (before, target, after, position);
        }

        /// <summary>
        /// 文字列内にある2つの特定の文字列を最後から探し出し、それらの文字の位置を知り、文字列をその位置で分割する関数
        /// </summary>
        public static (string Before, string Between, string After, int Position1, int Position2) SplitStringAtTwoTargets(string str, string target1, string target2)
        {
            int position2 = str.LastIndexOf(target2, StringComparison.Ordinal);

            if (position2 == -1)
            {
                return (str, null, null, -1, -1);
            }

            int position1 = str.LastIndexOf(target1, position2 - target1.Length, StringComparison.Ordinal);

            if (position1 == -1)
            {
                return (str.Substring(0, position2), null, str.Substring(position2 + target2.Length), -1, position2);
            }

            string before = str.Substring(0, position1);
            string between = str.Substring(position1 + target1.Length, position2 - position1 - target1.Length);
            string after = str.Substring(position2 + target2.Length);

            return (before, between, after, position1, position2);
        }
    }

    public class ParseResult
    {
        public string Type { get; set; }
        public int Station { get; set; }
        public string Name { get; set; }
        public bool Teihan { get; set; }
        public List<ParseResult> Execute { get; set; }
        public bool Sokatsu { get; set; }
        public bool Kata { get; set; }
    }

    public static class SaveData
    {
        public static Dictionary<string, TekoType> TekoTypeObj = new();
    }

    public class TekoType
    {
        public string Type { get; set; }
    }
}
