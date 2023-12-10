namespace ZLCEngine.EventSystem.MessageQueue
{
    /// <summary>
    ///     在主线程上运行的消息队列，依赖于Monobehaviour的Update方法
    /// </summary>
    public class MainThreadMQ : AMQ
    {
        private void Update()
        {
            if ((eventCount | taskCount) == 0) {
                return;
            }
            Act();
        }
    }

}