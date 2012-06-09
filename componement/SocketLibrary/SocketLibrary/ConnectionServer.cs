using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Dzw
{
    namespace SocketLevel
    {
        public class ConnectionServer : BaseSocket
        {
            public ConnectionServer(int UserTokenBufferLength):base(UserTokenBufferLength)
            {
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            public override Socket socket
            {
                get
                {
                    return this.m_mySocket;
                }
                set
                {
                    m_mySocket = value;
                }
            }

            //链接服务器
            public Boolean Connection(String ip, int port)
            {
                try
                {
                    m_mySocket.Connect(ip, port);
                }
                catch (System.Exception e)
                {
                    return false;
                }
                
                return m_mySocket.Connected;
            }

            public override Boolean isUse()
            {
                return m_mySocket.Connected;
            }
        }
    }
}
