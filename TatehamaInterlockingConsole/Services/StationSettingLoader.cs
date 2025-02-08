using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Services
{
    /// <summary>
    /// StationSettingList読込クラス
    /// </summary>
    public static class StationSettingLoader
    {
        /// <summary>
        /// TSVファイルを読み込みStationSettingListを作成する
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<List<StationSetting>> LoadSettings(string folderPath, string fileName)
        {
            try
            {
                // EncodingProviderを登録
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // ファイルパスを組み立てる
                string filePath = Path.Combine(folderPath, fileName);

                var Settings = new List<List<StationSetting>>();
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
                            StationNumber = columns[1],
                            FileName = columns[2],
                            ViewName = columns[3],
                            UpSideApproachingAlarmName = columns[4],
                            DownSideApproachingAlarmName = columns[5],
                            DirectionLeverAlarmName = columns[6]
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
    }
}
