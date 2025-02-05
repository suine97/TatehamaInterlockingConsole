using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Services
{
    /// <summary>
    /// ApproachingAlarmConditionList読込クラス
    /// </summary>
    public static class ApproachingAlarmSettingLoader
    {
        /// <summary>
        /// TSVファイルを読み込みApproachingAlarmSettingListを作成する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<List<ApproachingAlarmSetting>> LoadSettings(string folderPath, string fileName)
        {
            try
            {
                // ファイルパスを組み立てる
                string filePath = Path.Combine(folderPath, fileName);

                var Settings = new List<List<ApproachingAlarmSetting>>();
                bool header = false;
                foreach (var line in File.ReadAllLines(filePath, Encoding.GetEncoding("shift_jis")))
                {
                    // ヘッダー行はスキップ
                    if (!header)
                    {
                        header = true;
                        continue;
                    }

                    // 行に何も無ければスキップ
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var columns = line.Split('\t');

                    Settings.Add(new()
                    {
                        new()
                        {
                            StationName = columns[0],
                            OtherStationNameA = columns[1],
                            OtherStationNameB = columns[2],
                            IsUpSide = (columns[3] == "UP"),
                            Track = new()
                            {
                                Name = FormatName(columns[4]),
                                Station = GetStation(columns[4], columns),
                                IsReversePosition = IsReversePosition(columns[4]),
                                Type = GetType(columns[4])
                            },
                            ConditionsList = columns[5].Split(':').Select(condition => new ApproachingAlarmType
                            {
                                Name = FormatName(condition),
                                Station = GetStation(condition, columns),
                                IsReversePosition = IsReversePosition(condition),
                                Type = GetType(condition)
                            }).ToList()
                        }
                    });
                }
                return Settings;
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.ToString(), "エラー");
                return null;
            }
        }

        /// <summary>
        /// 所属駅名判定
        /// </summary>
        /// <param name="input"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static string GetStation(string input, string[] columns)
        {
            if (input.StartsWith("[[") && input.EndsWith("]]"))
            {
                return columns[2];
            }
            else if (input.StartsWith("[") && input.EndsWith("]"))
            {
                return columns[1];
            }
            else
            {
                return columns[0];
            }
        }

        /// <summary>
        /// 種別判定
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetType(string input)
        {
            var strName = FormatName(input);

            if (strName.EndsWith("T"))
            {
                return "Track";
            }
            else if (input.Contains("L") || strName.Contains("R"))
            {
                return "Signal";
            }
            else if (int.TryParse(strName, out _))
            {
                return "Point";
            }
            else
            {
                return "None";
            }
        }

        /// <summary>
        /// 反位判定
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool IsReversePosition(string input)
        {
            string strName = FormatName(input, true);
            return strName.StartsWith("(") && strName.EndsWith(")");
        }

        /// <summary>
        /// 名称整形
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FormatName(string input, bool isReverseCheck = false)
        {
            if (isReverseCheck)
            {
                return input.Replace("[[", "").Replace("]]", "").Replace("[", "").Replace("]", "");
            }
            else
            {
                return input.Replace("[[", "").Replace("]]", "").Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", "");
            }
        }
    }
}
