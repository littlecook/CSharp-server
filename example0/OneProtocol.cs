using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dzw.SocketLevel;
using Dzw.DataLevel;

namespace TestMyCom
{
    //一个协议实现
    class OneProtocol : IProtocol
    {
        private User m_cc;
        private byte[] m_buffer;        //数据缓冲区
        private int m_endIndex;         //数据结束位置

        private int m_postion;          //当前解析位置

        public OneProtocol(User cc)
        {
            m_cc = cc;
            m_buffer = new byte[ServerConfig.BUFFERLENGTH*10];
        }

        public void init()
        {
            m_endIndex = 0;
            m_postion = 0;
        }

        //socket层推入数据
        public void pushData(byte[] b, int length)
        {
            if ((m_endIndex - m_postion + length) > m_buffer.Length)    //*\\*/如果已有数据加上待加数据超过缓冲区长度则关闭此链接
            {
                Console.WriteLine("洪泛滥");
                m_cc.close();
            }
            else
            {
                //如果加入数据后会超过缓冲区末端,则先将之前的数据提前到0位置开始,再加入数据
                if ((m_endIndex + length) > m_buffer.Length)
                {
                    int residualLength = m_endIndex - m_postion;    //现有数据的长度
                    for (int i = 0; i < residualLength; i++)        //复制到0位置开始
                    {
                        m_buffer[i] = m_buffer[m_postion + i];
                    }
                    m_postion = 0;
                    m_endIndex = residualLength;
                }

                for (int i = 0; i < length; i++)
                {
                    m_buffer[i + m_endIndex] = b[i];
                }

                m_endIndex += length;
                parse();
            }
        }

        //数据解析
        public void parse()
        {
            byte packageLength = DataPro.getByte(m_buffer, ref m_postion);
            if ((packageLength + m_postion) <= m_endIndex)
            {
                Console.WriteLine("Complete Package");
                byte[] data = new byte[packageLength];
                DataPro.getByteArray(m_buffer, ref m_postion, data);

                ThreadMessage tm = new ThreadMessage();
                tm.data = data;
                tm.mc = m_cc;

                //推入线程池
                MyGlobal.mpool.addJob(tm);

                parse();
            }
            else
            {
                m_postion--;
            }
        }
    }
}
