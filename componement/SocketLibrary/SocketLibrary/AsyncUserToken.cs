using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Dzw
{
    namespace SocketLevel
    {
        public class AsyncUserToken
        {
            private Socket m_socket = null;
            private Byte[] m_buffer;
            private int m_index;

            public AsyncUserToken(int length)
            {
                m_index = 0;
                m_socket = null;
                m_buffer = new Byte[length];
            }

            public void init()
            {
                m_index = 0;
            }
            public int length
            {
                get{
                    return this.m_index;
                }
                
            }
            public Socket socket
            {
                set { this.m_socket = value; }
                get { return this.m_socket; }
            }
            public void addBytes(byte[] source, int length)
            {
                for (int i = 0; i < length; i ++ )
                {
                    m_buffer[m_index++] = source[i];
                }
            }

            public Byte[] buffer
            {
                get
                {
                    return this.m_buffer;
                }
            }
        }
    }
}
