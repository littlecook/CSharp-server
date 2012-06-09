using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMyCom
{
    class PacketDefine
    {
        public const byte UserIn = 0x01;
        public const  byte ConnectionSucess = 0x02;
        public const  byte UserLeave = 0x03;
        public const  byte EnterMap = 0x04;
        public const  byte PlayerInMap = 0x05;

        public const byte MoveInMap = 0x06;

        public const byte PlayerPositonMove = 0x07;
    }
}
