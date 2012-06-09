using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Dzw
{
    namespace SocketLevel
    {
        public class ServerSocket
        {
            private Socket m_listenSocket = null;
            private SocketAsyncEventArgs m_acceptEventArg = null;
            private Boolean m_serverState = false;
            private ManualResetEvent allDone = new ManualResetEvent(false);

            //所有客户端链接类
            public Stack<ClientConnection> m_clientStatck = null;
            public int m_clientNumber = 0;

            //初始化客户端链接个数
            public void Init(int clientNumber)
            {
                m_clientStatck = new Stack<ClientConnection>(m_clientNumber = clientNumber);

            }

            public Boolean state
            {
                set
                {

                }
                get
                {
                    return m_serverState;
                }
            }
            //加入派生客户端链接类对象
            public void AddClient(ClientConnection c)
            {
                if (m_clientStatck == null)
                {
                    return;
                }
                m_clientStatck.Push(c);
            }

            //开始服务器
            public int startListen(byte[] ip, int port)
            {
                if (m_clientStatck == null)
                {
                    return -1;
                }
                IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(ip), port);

                //初始化监听socket
                m_listenSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_listenSocket.Bind(ipEndPoint);
                m_listenSocket.Listen(ipEndPoint.Port);

                //生成一个线程
                Thread listenThread = new Thread(ServerSocket.Listen);
                listenThread.Start(this);
                return 1;
            }

            //线程函数
            public static void Listen(object o)
            {
                ServerSocket pThis = (ServerSocket)o;
                pThis.accept();
            }

            //侦听链接
            public void accept()
            {
                m_serverState = true;
                m_acceptEventArg = new SocketAsyncEventArgs();
                m_acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
                while (m_serverState)
                {
                    allDone.Reset();
                    m_acceptEventArg.AcceptSocket = null;
                    bool willRaiseEvent = m_listenSocket.AcceptAsync(m_acceptEventArg);
                    if (!willRaiseEvent)
                    {
                        ProcessAccept(m_acceptEventArg);
                    }
                    else
                    {
                        allDone.WaitOne();
                    }
                }
            }

            private void ProcessAccept(SocketAsyncEventArgs e)
            {
                if (m_clientStatck.Count == 0)
                {
                    return;
                }
                foreach (ClientConnection cc in m_clientStatck)
                {
                    if (!cc.isUse())
                    {
                        cc.socket = e.AcceptSocket;
                        allDone.Set();
                        break;
                    }
                }
            }

            //处理新链接
            private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
            {
                ProcessAccept(e);
            }
        }
    }
}
