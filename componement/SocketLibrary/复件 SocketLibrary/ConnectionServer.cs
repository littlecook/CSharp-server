using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Dzw
{
    namespace SocketLevel
    {
        public class ConnectionServer
        {
            private Socket m_connectionServer = null;
            protected SocketAsyncEventArgs m_ReadEventArg;
            protected Stack<SocketAsyncEventArgs> m_writeEventArgPool;
            protected IProtocol m_p = null;

            public ConnectionServer()
            {
                createProtocol();

                m_connectionServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);
                m_ReadEventArg = makeSocketAsyncEventArgs();
                int defaultLength = 8;
                for (int i = 0; i < defaultLength; i++)
                {
                    m_writeEventArgPool.Push(makeSocketAsyncEventArgs());
                }
            }

            //模板方法产生协议类
            protected virtual void createProtocol()
            {
                //m_p = new SimpleProtocol(this);
            }

            //生成自定义AsyncEventArg
            private SocketAsyncEventArgs makeSocketAsyncEventArgs()
            {
                int bufferLength = 300;
                SocketAsyncEventArgs re = new SocketAsyncEventArgs();
                re.UserToken = new AsyncUserToken(bufferLength);
                re.SetBuffer(((AsyncUserToken)re.UserToken).buffer, 0, bufferLength);
                re.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                return re;
            }
            void IO_Completed(object sender, SocketAsyncEventArgs e)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        //ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        //ProcessSend(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }

            }
            public void Close()
            {

            }
            //链接服务器
            public Boolean Connection(String ip, int port)
            {
                m_connectionServer.Connect(ip, port);
                return m_connectionServer.Connected;
            }

            //异步写入
            public void AsyncWrite(byte[] source, byte length)
            {
                SocketAsyncEventArgs writeEventArg = getAsyncEventArgs();
                AsyncUserToken au = (AsyncUserToken)writeEventArg.UserToken;
                au.addBytes(source, length);
                this.asyncWrite(writeEventArg);
            }

            //获取异步参数
            public SocketAsyncEventArgs getAsyncEventArgs()
            {
                SocketAsyncEventArgs wirteEvent = null;
                if (m_writeEventArgPool.Count == 0)
                {
                    wirteEvent = makeSocketAsyncEventArgs();
                }
                else
                {
                    wirteEvent = m_writeEventArgPool.Pop();

                }

                ((AsyncUserToken)wirteEvent.UserToken).socket = m_connectionServer;
                return wirteEvent;
            }
            private void asyncWrite(SocketAsyncEventArgs writeEvent)
            {
                writeEvent.SetBuffer(0, ((AsyncUserToken)writeEvent.UserToken).length);
                m_connectionServer.SendAsync(writeEvent);
            }
            public void AsyncRead()
            {
                bool willRaiseEvent = m_connectionServer.ReceiveAsync(m_ReadEventArg);
                if (!willRaiseEvent)
                {
                    ProcessReceive(m_ReadEventArg);
                }
            }

            private void ProcessReceive(SocketAsyncEventArgs e)
            {
                //读取成功
                Console.WriteLine("read data length:" + e.BytesTransferred);
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    m_p.pushData(e.Buffer, e.BytesTransferred);
                    AsyncRead();
                }
                else
                {
                    Close();
                }
            }
        }
    }
}
