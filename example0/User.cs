using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dzw.SocketLevel;
using Dzw.DataLevel;
using System.IO;

namespace TestMyCom
{
    class User : ClientConnection
    {
        public static byte[] crossdata = new byte[256];
        static int UserID = 45;

        public Map m_map = null;
        int tempindex = 0;
        int m_id = 0;
        public int x = 0;
        public int y = 0;

        private Boolean isON = false;

        public User(int length) :base(length)
        {

        }
        //客户端链接
        public override void onConnection()
        {
            m_id = User.UserID++;

            Console.WriteLine("发送cross data");
            asyncWrite(User.crossdata, User.crossdata.Length);
            return;
            //发送ID
           
        }
        public override void close()
        {
            if (isON)
            {
                m_mySocket.Close();
                m_mySocket = null;
                onDisConnection();
            }
            isON = false;
        }
        //客户端断开链接
        protected override void onDisConnection()
        {
            if (m_map != null)
            {
                //从map中移除这个玩家
                m_map.Leave(this);
            }
            m_map = null;
        }

        public int id
        {
            get
            {
                return this.m_id;
            }
        }
        public void sendPacket(Packet p)
        {
            p.end();
            base.asyncWrite(p.buffer, p.length);
        }
        protected override void createProtocol()
        {
            m_p = new OneProtocol(this);
        }

        public void doData(byte[] data)
        {
            tempindex = 0;
            byte command = DataPro.getByte(data, ref tempindex);
            switch(command)
            {
                case PacketDefine.UserIn:
                    isON = true;
                    Packet p = new Packet();
                    p.addByte(PacketDefine.ConnectionSucess);
                    p.addInt(m_id);
                    Console.WriteLine("玩家： {0}", m_id);
                    sendPacket(p);
                    break;
                case PacketDefine.EnterMap:
                    Enter(data);
                    break;
                case PacketDefine.MoveInMap:
                    Move(data);
                    break;
            }

        }

        protected void Enter(byte[] data)
        {
            int mapId = DataPro.getInt(data, ref tempindex);
            Map m = MapManager.getMapOfId(mapId);
            if (m != null)
            {
                m_map = m;
                x = DataPro.getInt(data, ref tempindex);
                y = DataPro.getInt(data, ref tempindex);
                m_map.Enter(this);
            }
        }
        protected void Move(byte[] data)
        {
            int mapId = DataPro.getInt(data, ref tempindex);
            
            if (m_map.id == mapId)
            {
                x = DataPro.getInt(data, ref tempindex);
                y = DataPro.getInt(data, ref tempindex);

                m_map.Move(this);
            }
        }
    }
}
