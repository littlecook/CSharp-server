using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dzw
{
    namespace SocketLevel
    {
        //解包协议接口

        public interface IProtocol
        {
            void pushData(byte[] data, int length);
            void init();
            void parse();
        }
    }
}
