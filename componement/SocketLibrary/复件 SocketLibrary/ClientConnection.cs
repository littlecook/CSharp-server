using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Dzw
{
    namespace SocketLevel
    {
        //客户端链接
        public class ClientConnection : BaseSocket
        {
            //客户端socket
            protected Socket m_mySocket = null;
            //读
            protected SocketAsyncEventArgs m_ReadEventArg;
            //写队列
            protected Stack<SocketAsyncEventArgs> m_writeEventArgPool;
            //协议解析
            protected IProtocol m_p = null;

            public ClientConnection()
            {
                createProtocol();

                m_ReadEventArg = makeSocketAsyncEventArgs();
                int defaultLength = 5;
                m_writeEventArgPool = new Stack<SocketAsyncEventArgs>(defaultLength);
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

            public Socket socket
            {
                get
                {
                    return this.m_mySocket;
                }
                set
                {
                    m_mySocket = value;
                    ((AsyncUserToken)m_ReadEventArg.UserToken).socket = m_mySocket;
                    
                    asyncRead();
                    onConnection();
                }
            }

            //新到链接
            public virtual void onConnection()
            {
                System.Console.WriteLine("ClientConection onConnection");
            }

            //断开链接
            protected virtual void onDisConnection()
            {

            }

            //发出异步读取
            public void asyncRead()
            {
                bool willRaiseEvent = m_mySocket.ReceiveAsync(m_ReadEventArg);
                if (!willRaiseEvent)
                {
                    ProcessReceive(m_ReadEventArg);
                }
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

                ((AsyncUserToken)wirteEvent.UserToken).socket = m_mySocket;
                return wirteEvent;
            }

            //异步写入字节流
            public void asyncWrite(byte[] source, int length)
            {
                SocketAsyncEventArgs writeEventArg = getAsyncEventArgs();
                AsyncUserToken au = (AsyncUserToken)writeEventArg.UserToken;
                au.addBytes(source, length);
                this.asyncWrite(writeEventArg);
            }

            //发出异步写入
            private void asyncWrite(SocketAsyncEventArgs writeEvent)
            {
                writeEvent.SetBuffer(0, ((AsyncUserToken)writeEvent.UserToken).length);
                m_mySocket.SendAsync(writeEvent);
            }

            //IO完成通知
            void IO_Completed(object sender, SocketAsyncEventArgs e)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    default:
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }

            }

            //处理读取完成
            private void ProcessReceive(SocketAsyncEventArgs e)
            {
                //读取成功
                Console.WriteLine("read data length:" + e.BytesTransferred);
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    m_p.pushData(e.Buffer, e.BytesTransferred);
                    asyncRead();
                }
                else
                {
                    close();
                }
            }

            //关闭客户端
            public virtual void close()
            {
                if (m_p != null)
                {
                    this.m_p.init();
                }
                this.m_mySocket.Close();
                this.m_mySocket = null;
                onDisConnection();
            }

            public Boolean isUse()
            {
                return this.m_mySocket != null;
            }

            //处理发送完成
            private void ProcessSend(SocketAsyncEventArgs e)
            {
                Console.WriteLine("send sucess ");
                m_writeEventArgPool.Push(e);
                ((AsyncUserToken)e.UserToken).init();
            }
        }
    }
}
