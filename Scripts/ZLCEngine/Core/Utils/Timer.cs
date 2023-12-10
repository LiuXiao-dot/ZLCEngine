using System;
namespace ZLCEngine.Utils
{
    /// <summary>
    ///     倒计时部分控制
    /// </summary>
    public sealed class Timer : IDisposable
    {
        private readonly Action onFinish;
        private readonly Action onUpdate;
        private readonly Action update;
        private int remainingTime;
        /// <summary>
        /// </summary>
        /// <param name="onUpdate"></param>
        /// <param name="onFinish"></param>
        public Timer(Action onUpdate, Action onFinish)
        {
            this.onUpdate = onUpdate;
            this.onFinish = onFinish;
            update = Update;
        }
        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
        }
        /// <summary>
        ///     开始计时
        /// </summary>
        private void Start()
        {
            CoroutineHelper.AddCoroutineWaitTime(update, -1, 1);
        }

        /// <summary>
        ///     停止计时
        /// </summary>
        private void Stop()
        {
            CoroutineHelper.StopCoroutine(update);
        }

        /// <summary>
        ///     设置剩余倒计时时间
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(int time)
        {
            remainingTime = time;
        }

        /// <summary>
        ///     每秒触发更新
        /// </summary>
        private void Update()
        {
            remainingTime--;
            onUpdate?.Invoke();
            if (remainingTime < 0) {
                // 倒计时结束
                Stop();
                onFinish?.Invoke();
            }
        }

        /// <summary>
        /// </summary>
        ~Timer()
        {
            Dispose();
        }
    }

}