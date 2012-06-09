using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dzw.ThreadLevel;

namespace TestMyCom
{
    class MyWorkerThread : DzwWorkerThread<ThreadMessage>
    {
        protected override void excute()
        {
            m_instance.mc.doData(m_instance.data);
        }
    }
    class MyThreadPool : DzwThreadPool<ThreadMessage>
    {
        public MyThreadPool(int length)
            : base(length)
        {
        }
        protected override DzwWorkerThread<ThreadMessage> createWorkerThread()
        {
            return new MyWorkerThread();
        }
    }

    class MyGlobal
    {
        public static MyThreadPool mpool = new MyThreadPool(50);
    }

}
