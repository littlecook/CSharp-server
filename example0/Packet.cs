using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dzw.DataLevel;

namespace TestMyCom
{
    class Packet
    {
        private byte[] m_buffer = new byte[ServerConfig.BUFFERLENGTH];
        private int index = 1;

        public int length
        {
            get
            {
                return this.index;
            }
        }

        public byte[] buffer
        {
            get
            {
                return this.m_buffer;
            }
        }
        public void addInt(int data)
        {
            DataPro.addData(m_buffer, ref index, data);
        }

        public void addByte(byte data)
        {
            DataPro.addData(m_buffer, ref index, data);
        }

        public void addString(String data)
        {
            DataPro.addData(m_buffer, ref index, data);
        }
        public void end()
        {
            m_buffer[0] = (byte)((byte)index - 0x01);
        }
    }
}
