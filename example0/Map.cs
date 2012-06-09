using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMyCom
{
    class Map
    {
        private object mylock = new object();

        public int id;
        public List<User> m_stack = new List<User>();

        public void Leave(User u)
        {
            Packet p = new Packet();
            p.addByte(PacketDefine.UserLeave);
            p.addInt(u.id);
            p.addInt(this.id);

            foreach (User user in m_stack)
            {
                if (user == u)
                {
                    lock(mylock)
                    {
                        m_stack.Remove(user);
                        break;
                    }
                }
            }
            foreach (User ua in m_stack)
            {
                ua.sendPacket(p);
            }
        }

        public void Enter(User u)
        {
            lock (mylock)
            {
                m_stack.Add(u);
            }

            Packet p = new Packet();
            p.addByte(PacketDefine.PlayerInMap);
            p.addInt(u.id);
            p.addInt(this.id);
            p.addInt(u.x);
            p.addInt(u.y);

            foreach (User user in m_stack)
            {
                user.sendPacket(p);         //向玩家发送此玩家进入地图

                Packet other = new Packet();
                other.addByte(PacketDefine.PlayerInMap);
                other.addInt(user.id);
                other.addInt(this.id);
                other.addInt(user.x);
                other.addInt(user.y);

                if (user.id != u.id)
                {
                    u.sendPacket(other);    //向u发送已经存在地图的玩家
                }
                
            }
        }

        public void Move(User u)
        {
            Packet p = new Packet();
            p.addByte(PacketDefine.PlayerPositonMove);
            p.addInt(u.id);
            p.addInt(this.id);
            p.addInt(u.x);
            p.addInt(u.y);

            foreach (User user in m_stack)
            {
                user.sendPacket(p);
            }
        }
    }

    class MapManager
    {
        private static Stack<Map> m_stack = new Stack<Map>();

        public static void init(int lenght)
        {
            for (int i = 0; i < lenght; i ++ )
            {
                Map m = new Map();
                m.id = i;
                m_stack.Push(m);
            }
        }

        public static Map getMapOfId(int id)
        {
            foreach (Map m in m_stack)
            {
                if (m.id == id)
                {
                    return m;
                }
            }
            return null;
        }
    }
}
