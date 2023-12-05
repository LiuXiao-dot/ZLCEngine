using System.Threading;
using ZLCEngine.ThreadSystem;
using ThreadPool = ZLCEngine.ThreadSystem.ThreadPool;
namespace ZLCEngine.EventSystem.MessageQueue
{
    /// <summary>
    /// 子线程上运行的消息队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ChildThreadMQ : AMQ
    {
        private Thread thread;
        private ThreadWrapper threadWrapper;
        private void Awake()
        {
            thread = ThreadPool.Instance.Obtain();
            threadWrapper = ThreadPool.Instance.GetWrapper(thread);
            threadWrapper.JoinCover(Run);
        }

        /// <inheritdoc />
        public override void SendEvent(int operate, object args)
        {
            base.SendEvent(operate, args);
            if (thread.ThreadState == ThreadState.Unstarted) {
                thread.Start();
            }
            threadWrapper.Resume();
        }

        private void Run()
        {
            while (true) {
                if ((eventCount | taskCount) == 0) {
                    threadWrapper.Pause();
                    continue;
                }
                Act();
            }
        }
    }
}