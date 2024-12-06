using NAudio.Wave;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TatehamaInterlockingConsole.Helpers;

namespace TatehamaInterlockingConsole.Services
{
    /// <summary>
    /// 音声クラス
    /// </summary>
    public class Sound
    {
        private static Sound _instance = new Sound();
        /// <summary>
        /// インスタンス生成
        /// </summary>
        public static Sound Instance => _instance;

        /// <summary>
        /// 音声リスト
        /// </summary>
        public List<SoundFile> SoundList;

        /// <summary>
        /// 音声処理Queue
        /// </summary>
        public ConcurrentQueue<SoundPlayArgument> SoundQueue = new ConcurrentQueue<SoundPlayArgument>();

        /// <summary>
        /// 再生中のデバイスリスト
        /// </summary>
        private readonly List<WaveOutEvent> activeDevices = new List<WaveOutEvent>();

        /// <summary>
        /// 音声処理スレッドループ判定
        /// </summary>
        public bool IsSoundThreadLoop = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Sound()
        {
            // 音声ファイル読み込み
            SoundList = ReadAudioFile();
        }

        /// <summary>
        /// 音声処理スレッド
        /// </summary>
        public async void SoundThread()
        {
            while (IsSoundThreadLoop)
            {
                // 50msごとに処理
                await Task.Delay(50);

                // Queueに処理が積まれた時に実行
                if (SoundQueue.Count == 0)
                    continue;

                // Queueからデータ取得
                if (SoundQueue.TryDequeue(out SoundPlayArgument s))
                {
                    SoundPlay(s.sFileName, s.IsLoop);
                }
            }
        }

        /// <summary>
        /// 音声再生メソッド（複数音声対応）
        /// </summary>
        public void SoundPlay(string soundFileName, bool isLoop)
        {
            try
            {
                // 音声ファイルの検索
                SoundFile sFile = SoundList.First(s => s.sFileName == soundFileName);
                AudioFileReader audioReader = new AudioFileReader(sFile.sFilePath);

                // 出力デバイスの生成
                WaveOutEvent device = new WaveOutEvent();
                device.PlaybackStopped += (sender, e) =>
                {
                    // 再生停止後にリソース解放
                    device.Dispose();
                    audioReader.Dispose();
                    lock (activeDevices)
                    {
                        activeDevices.Remove(device);
                    }
                };

                if (isLoop)
                {
                    LoopStream loop = new LoopStream(audioReader);
                    device.Init(loop);
                }
                else
                {
                    device.Init(audioReader);
                }

                // 再生実行
                device.Play();

                // 再生中のデバイスを管理
                lock (activeDevices)
                {
                    activeDevices.Add(device);
                }
            }
            catch
            {
                // エラー時のリソース解放処理
                SoundStopAll();
                SoundQueue.Enqueue(new SoundPlayArgument() { sFileName = soundFileName, IsLoop = isLoop });
            }
        }

        /// <summary>
        /// 指定した音声を停止
        /// </summary>
        public void SoundStop(string soundFileName)
        {
            lock (activeDevices)
            {
                // 対応するデバイスを検索
                var deviceToStop = activeDevices.FirstOrDefault(device =>
                {
                    if (device.PlaybackState == PlaybackState.Playing)
                    {
                        // 音声ファイルパスを確認
                        var reader = ((WaveStream)device.GetType()
                            .GetField("waveStream", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                            .GetValue(device)) as AudioFileReader;
                        return reader?.FileName == SoundList.FirstOrDefault(s => s.sFileName == soundFileName)?.sFilePath;
                    }
                    return false;
                });

                if (deviceToStop != null)
                {
                    // 停止してリストから削除
                    deviceToStop.Stop();
                    deviceToStop.Dispose();
                    activeDevices.Remove(deviceToStop);
                }
            }
        }

        /// <summary>
        /// 全ての音声を停止
        /// </summary>
        public void SoundStopAll()
        {
            lock (activeDevices)
            {
                foreach (var device in activeDevices)
                {
                    device.Stop();
                }
                activeDevices.Clear();
            }
        }

        /// <summary>
        /// 音声ファイル読み込みメソッド
        /// </summary>
        private List<SoundFile> ReadAudioFile()
        {
            // Sound配下のwavファイルのパスを全取得
            string[] filePaths = Directory.GetFiles(DataHelper.GetApplicationDirectory() + "\\Sound", "*.wav", SearchOption.TopDirectoryOnly);

            // 音声ファイル情報を読み込んでListに入れる
            List<SoundFile> soundList = new List<SoundFile>();
            foreach (string filePath in filePaths)
            {
                SoundFile audioFile = new SoundFile()
                {
                    sFileName = Path.ChangeExtension(Path.GetFileName(filePath), null),
                    sFilePath = filePath
                };
                soundList.Add(audioFile);
            }
            return soundList;
        }

        /// <summary>
        /// 音声ファイルクラス
        /// </summary>
        public class SoundFile
        {
            /// <summary>
            /// ファイル名
            /// </summary>
            public string sFileName { get; set; }

            /// <summary>
            /// ファイルパス
            /// </summary>
            public string sFilePath { get; set; }
        }

        /// <summary>
        /// 音声再生用クラス
        /// </summary>
        public class SoundPlayArgument
        {
            /// <summary>
            /// ファイル名
            /// </summary>
            public string sFileName { get; set; }

            /// <summary>
            /// ループ判定
            /// </summary>
            public bool IsLoop { get; set; }
        }

        /// <summary>
        /// ループ再生用クラス
        /// </summary>
        public class LoopStream : WaveStream
        {
            private readonly WaveStream sourceStream;

            public LoopStream(WaveStream sourceStream)
            {
                this.sourceStream = sourceStream;
                this.EnableLooping = true;
            }

            public bool EnableLooping { get; set; }

            public override WaveFormat WaveFormat => sourceStream.WaveFormat;

            public override long Length => sourceStream.Length;

            public override long Position
            {
                get => sourceStream.Position;
                set => sourceStream.Position = value;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int totalBytesRead = 0;

                while (totalBytesRead < count)
                {
                    int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        if (sourceStream.Position == 0 || !EnableLooping)
                        {
                            break;
                        }
                        sourceStream.Position = 0;
                    }
                    totalBytesRead += bytesRead;
                }
                return totalBytesRead;
            }
        }
    }
}
