using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dzw.SocketLevel;
using Dzw.ThreadLevel;
using System.IO;

namespace TestMyCom
{
    class myClient : ConnectionServer
    {
        public myClient(int num) : base(num)
        {

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            myClient mc = new myClient(300);
            Boolean bcli = mc.Connection("127.0.0.1", 4567);
            Console.WriteLine("链接状态: " + bcli);
            FileStream sf = File.OpenRead("crossdomain.xml");
            //Flash.crossdata = new byte[sf.Length + 1];
            sf.Read(User.crossdata, 0, (int)sf.Length);
            User.crossdata[sf.Length] = (byte)0;

            MapManager.init(13);         //初始化三张地图

            ServerSocket ss = new ServerSocket();
            ss.Init(ServerConfig.CLIENTNUMBER);
            for (int i = 0; i < ServerConfig.CLIENTNUMBER; i++)
            {
                ss.AddClient(new User(300));
            }
            ss.startListen(ServerConfig.SERVER_IP, ServerConfig.SERVER_PORT);
        }
    }
}
