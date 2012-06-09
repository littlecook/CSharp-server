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
            public ClientConnection(int UserTokenBufferLength) : base(UserTokenBufferLength)
            {
                
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
                    ((AsyncUserToken)m_ReadEventArg.UserToken).socket = m_mySocket;
                    
                    asyncRead();
                    onConnection();
                }
            }


            public override Boolean isUse()
            {
                return this.m_mySocket != null;
            }
        }
    }
}
