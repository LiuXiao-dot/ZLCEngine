namespace ZLCEngine.EventSystem.MessageQueue
{
    /// <summary>
    ///     在主线程上运行的消息队列，依赖于Monobehaviour的Update方法
    /// </summary>
    public class MainThreadMQ : AMQ
    {
        public override void SendEvent(int operate, object args)
        {
            base.SendEvent(operate, args);
            enabled = true;
        }

        private void Update()
        {
            if ((eventCount | taskCount) == 0) {
                enabled = false;
                return;
            }
            Act();
        }
    }

}