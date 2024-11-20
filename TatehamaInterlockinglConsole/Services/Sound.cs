using NAudio.Wave;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TatehamaInterlockinglConsole.Services
{
    /// <summary>
    /// 音声クラス
    /// </summary>
    public class Sound
    {
        /// <summary>
        /// 音声リスト
        /// </summary>
        public List<SoundFile> SoundList;

        /// <summary>
        /// 音声処理Queue
        /// </summary>
        public ConcurrentQueue<SoundPlayArgument> SoundQueue = new ConcurrentQueue<SoundPlayArgument>();

        /// <summary>
        /// 音声再生用の出力デバイス
        /// </summary>
        private WaveOutEvent outputDevice;

        /// <summary>
        /// 音声読み込み
        /// </summary>
        private AudioFileReader audioFile;

        /// <summary>
        /// 音声処理スレッドループ判定
        /// </summary>
        public bool IsSoundThreadLoop = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Sound()
        {
            //音声ファイル読み込み
            SoundList = ReadAudioFile();
        }

        /// <summary>
        /// 音声処理スレッド
        /// </summary>
        public async void SoundThread()
        {
            while (IsSoundThreadLoop)
            {
                //50msごとに処理
                await Task.Delay(50);

                //Queueに処理が積まれた時に実行
                if (SoundQueue.Count() == 0)
                    continue;

                //何も再生されていない時に処理
                if (outputDevice != null)
                    continue;

                //Queueからデータ取得
                if (SoundQueue.TryDequeue(out SoundPlayArgument s))
                {
                    SoundPlay(s.sFileName, s.IsLoop);
                }
            }
        }

        /// <summary>
        /// 音声再生メソッド
        /// </summary>
        public void SoundPlay(string soundFileName, bool isLoop)
        {
            try
            {
                if (outputDevice == null)
                {
                    //出力デバイスの生成
                    outputDevice = new WaveOutEvent();
                    //stop()を実行すると発生　クリーンアップ処理等を行う
                    outputDevice.PlaybackStopped += OnPlaybackStopped;
                    if (audioFile == null)
                    {
                        //音声ファイルの検索
                        SoundFile sFile = SoundList.First(s => s.sFileName == soundFileName);
                        //音声ファイルの読み込み
                        audioFile = new AudioFileReader(sFile.sFilePath);
                        //ループ用処理
                        if (isLoop)
                        {
                            //ループ設定取得
                            LoopStream loop = new LoopStream(audioFile);
                            //出力デバイスに渡す
                            outputDevice.Init(loop);
                        }
                        else
                        {
                            //出力デバイスに渡す
                            outputDevice.Init(audioFile);
                        }
                    }
                    //再生実行
                    outputDevice.Play();
                }
            }
            catch
            {
                SoundStop();
                //Queteに再設定
                SoundQueue.Enqueue(new SoundPlayArgument() { sFileName = soundFileName, IsLoop = isLoop });
            }
        }

        /// <summary>
        /// 音声停止メソッド
        /// </summary>
        public void SoundStop()
        {
            outputDevice?.Stop();
        }

        /// <summary>
        /// 音声ファイル読み込みメソッド
        /// </summary>
        private List<SoundFile> ReadAudioFile()
        {
            //Sound配下のwavファイルのパスを全取得
            string[] filePaths = Directory.GetFiles(Data.GetAppPath() + "\\Sound", "*.wav", SearchOption.TopDirectoryOnly);

            //音声ファイル情報を読み込んでListに入れる
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
        /// 音声停止イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            //出力デバイス破棄
            outputDevice.Dispose();
            outputDevice = null;
            //オーディオファイル破棄
            audioFile.Dispose();
            audioFile = null;
        }
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
        WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
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
