using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatehamaInterlockinglConsole.Models
{
    /// <summary>
    /// 連動装置サーバーデータ格納クラス
    /// </summary>
    public class ServerData
    {
        /// <summary>
        /// 連動装置・コマンド
        /// </summary>
        public class CommandToServer
        {
            public string Command { get; set; }
            public string[] Args { get; set; }
        }
    }
}
