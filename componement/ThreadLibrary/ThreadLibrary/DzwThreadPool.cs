using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dzw
{
    namespace ThreadLevel
    {
        //线程池类
        public class DzwThreadPool<T>
        {
            private Thread m_thread;                        //线程池工作线程
            private Stack<T> m_jobStack = new Stack<T>();   //任务队列
            private Stack<DzwWorkerThread<T>> m_workerStack = new Stack<DzwWorkerThread<T>>();  //工作线程队列

            public DzwThreadPool(int length)
            {
                //初始化工作线程队列
                for (int i = 0; i < length; i++)
                {
                    m_workerStack.Push(createWorkerThread());
                }

                m_thread = new Thread(DzwThreadPool<T>.ThreadFunction);
                m_thread.Start(this);
            }

            protected virtual DzwWorkerThread<T> createWorkerThread()
            {
                return null;
            }
            static void ThreadFunction(object o)
            {
                DzwThreadPool<T> worker = (DzwThreadPool<T>)o;
                worker.run();
            }

            //线程池工作函数
            private void run()
            {
                while(true)
                {
                    if (m_jobStack.Count != 0)   //如果任务队列中含有未处理任务
                    {
                        foreach (DzwWorkerThread<T> dworker in m_workerStack)
                        {
                            if (m_jobStack.Count == 0) break;
                            if (dworker.state != ThreadState.Running)
                            {
                                dworker.doJob(m_jobStack.Pop());
                            }
                        }
                    }
                }
            }

            //加入任务到线程池
            public void addJob(T t)
            {
                m_jobStack.Push(t);
            }
        }
    }
}
