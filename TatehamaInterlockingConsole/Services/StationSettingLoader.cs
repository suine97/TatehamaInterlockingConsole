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
        public static List<StationSetting> LoadSettings(string folderPath, string fileName)
        {
            try
            {
                // EncodingProviderを登録
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // ファイルパスを組み立てる
                string filePath = Path.Combine(folderPath, fileName);

                var Settings = new List<StationSetting>();
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

                    Settings.Add(
                        new()
                        {
                            StationName = columns[0],
                            StationNumber = columns[1],
                            FileName = columns[2],
                            ViewName = columns[3],
                            UpSideAlarmName = columns[4] != string.Empty ? columns[1] + "_" + columns[4] : string.Empty,
                            UpSideAlarmType = columns[5],
                            DownSideAlarmName = columns[6] != string.Empty ? columns[1] + "_" + columns[6] : string.Empty,
                            DownSideAlarmType = columns[7],
                            DirectionAlarmName = columns[8] != string.Empty ? columns[1] + "_" + columns[8] : string.Empty,
                            DirectionAlarmType = columns[9]
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
