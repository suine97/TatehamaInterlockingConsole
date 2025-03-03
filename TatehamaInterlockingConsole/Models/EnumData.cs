using System;

namespace TatehamaInterlockingConsole.Models
{
    public static class EnumData
    {
        /// <summary>
        /// TSVファイルの列要素番号
        /// </summary>
        public enum ColumnIndex
        {
            ControlType = 0,
            UniqueName = 1,
            ParentName = 2,
            ServerType = 3,
            ServerName = 4,
            PointNameA = 5,
            PointValueA = 6,
            PointNameB = 7,
            PointValueB = 8,
            DirectionName = 9,
            DirectionValue = 10,
            X = 11,
            Y = 12,
            Width = 13,
            Height = 14,
            Angle = 15,
            AngleOriginX = 16,
            AngleOriginY = 17,
            Text = 18,
            FontSize = 19,
            BackgroundColor = 20,
            TextColor = 21,
            ClickEventName = 22,
            ImagePattern = 23,
            ImageIndex = 24,
            BaseImagePath = 25,
            ImagePath = 26,
            Remark = 27
        }

        /// <summary>
        /// 信号現示
        /// </summary>
        public enum Phase
        {
            None,
            R,
            YY,
            Y,
            YG,
            G
        }

        /// <summary>
        /// 転てつ器状態
        /// </summary>
        public enum NRC
        {
            Normal,
            Reversed,
            Center
        }

        /// <summary>
        /// 信号・転てつ器物理てこ状態
        /// </summary>
        public enum LCR
        {
            Left,
            Center,
            Right
        }

        /// <summary>
        /// 方向てこ状態
        /// </summary>
        public enum LNR
        {
            Left,
            Normal,
            Right
        }

        /// <summary>
        /// 物理ボタン状態
        /// </summary>
        public enum RaiseDrop
        {
            /// <summary>
            /// 離す
            /// </summary>
            Raise,
            /// <summary>
            /// 押す
            /// </summary>
            Drop
        }

        /// <summary>
        /// int型をNRC型に変換する
        /// </summary>
        /// <param name="value">変換するint値</param>
        /// <returns>変換されたLNR型</returns>
        public static NRC ConvertToNRC(int value)
        {
            if (value < 0)
                return NRC.Normal;
            else if (value == 0)
                return NRC.Center;
            else
                return NRC.Reversed;
        }
        /// <summary>
        /// NRC型をint型に変換する
        /// </summary>
        /// <param name="nrc">変換するNRC値</param>
        /// <returns>変換されたint値</returns>
        public static int ConvertFromNRC(NRC nrc)
        {
            switch (nrc)
            {
                case NRC.Normal:
                    return -1;
                case NRC.Center:
                    return 0;
                case NRC.Reversed:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nrc), nrc, null);
            }
        }

        /// <summary>
        /// int型をLCR型に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LCR ConvertToLCR(int value)
        {
            if (value < 0)
                return LCR.Left;
            else if (value == 0)
                return LCR.Center;
            else
                return LCR.Right;
        }
        /// <summary>
        /// LCR型をint型に変換する
        /// </summary>
        /// <param name="lcr">変換するLCR値</param>
        /// <returns>変換されたint値</returns>
        public static int ConvertFromLCR(LCR lcr)
        {
            switch (lcr)
            {
                case LCR.Left:
                    return -1;
                case LCR.Center:
                    return 0;
                case LCR.Right:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lcr), lcr, null);
            }
        }

        /// <summary>
        /// int型をLNR型に変換する
        /// </summary>
        /// <param name="value">変換するint値</param>
        /// <returns>変換されたLNR型</returns>
        public static LNR ConvertToLNR(int value)
        {
            if (value < 0)
                return LNR.Left;
            else if (value == 0)
                return LNR.Normal;
            else
                return LNR.Right;
        }
        /// <summary>
        /// LNR型をint型に変換する
        /// </summary>
        /// <param name="lnr">変換するLNR値</param>
        /// <returns>変換されたint値</returns>
        public static int ConvertFromLNR(LNR lnr)
        {
            switch (lnr)
            {
                case LNR.Left:
                    return -1;
                case LNR.Normal:
                    return 0;
                case LNR.Right:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lnr), lnr, null);
            }
        }

        /// <summary>
        /// int型をRaiseDrop型に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RaiseDrop ConvertToRaiseDrop(int value)
        {
            if (value == 1)
                return RaiseDrop.Raise;
            else
                return RaiseDrop.Drop;
        }
        /// <summary>
        /// RaiseDrop型をint型に変換する
        /// </summary>
        /// <param name="raiseDrop">変換するRaiseDrop値</param>
        /// <returns>変換されたint値</returns>
        public static int ConvertFromRaiseDrop(RaiseDrop raiseDrop)
        {
            switch (raiseDrop)
            {
                case RaiseDrop.Raise:
                    return 1;
                case RaiseDrop.Drop:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(raiseDrop), raiseDrop, null);
            }
        }
    }
}
