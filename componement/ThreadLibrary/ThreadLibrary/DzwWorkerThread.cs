using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace Dzw
{
    namespace ThreadLevel
    {
        //泛型工作线程类
        public class DzwWorkerThread<T>
        {
            protected Thread m_Thread = null;
            protected T m_instance;
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            public DzwWorkerThread()
            {
                m_Thread = new Thread(DzwWorkerThread<T>.ThreadFunction);
                m_Thread.Start(this);
            }

            public ThreadState state
            {
                get{
                    return m_Thread.ThreadState;
                }
            }
            static void ThreadFunction(object o)
            {
                DzwWorkerThread<T> worker = (DzwWorkerThread<T>)o;
                worker.run();
            }

            //工作函数
            private void run()
            {
                while (true)
                {
                    autoEvent.WaitOne();
                    excute();
                }
            }

            //执行业务
            protected virtual void excute()
            {

            }
            public void doJob(T t)
            {
                m_instance = t;
                autoEvent.Set();          //新业务到达后激活线程
            }
        }
    }
}
