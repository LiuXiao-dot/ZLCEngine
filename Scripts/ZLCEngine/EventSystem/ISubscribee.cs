using System;
using System.Collections.Generic;
namespace ZLCEngine.EventSystem
{
    /// <summary>
    ///     通用事件
    /// </summary>
    public struct Event
    {
        /// <summary>
        ///     事件的发送者
        ///     <remarks>在消息队列中，这个发送者是消息队列本身</remarks>
        /// </summary>
        public object sender;

        /// <summary>
        ///     事件类型
        /// </summary>
        public int operate;

        /// <summary>
        ///     参数
        /// </summary>
        public object data;

        /// <summary>
        /// 事件执行完毕后调用
        /// </summary>
        private Action<Event> callback;

        /// <summary>
        ///     事件基类的构造函数
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="operate">事件</param>
        /// <param name="data">事件的参数</param>
        /// <param name="callback">有些是异步执行完，所以需要自行调用</param>
        public Event(object sender, int operate, object data, Action<Event> callback)
        {
            this.sender = sender;
            this.operate = operate;
            this.data = data;
            this.callback = callback;
        }

        public void Callback()
        {
            this.callback.Invoke(this);
        }

        public override string ToString()
        {
            return $"sender:{sender.ToString()} operate:{operate} data:{data.ToString()}";
        }
    }

    /// <summary>
    ///     被订阅者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISubscribee<T>
    {
        /// <summary>
        ///     订阅事件
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <param name="operate">要订阅的事件ID</param>
        void Subscribe(ISubscriber subscriber, T operate);
        /// <summary>
        ///     订阅事件
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <param name="operates">要订阅的事件ID</param>
        void Subscribe(ISubscriber subscriber, IEnumerable<T> operates);
        /// <summary>
        ///     取消订阅
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <param name="operate">要订阅的事件</param>
        void Unsubscribe(ISubscriber subscriber, T operate);
        /// <summary>
        ///     取消订阅
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <param name="operates">要取消订阅的事件ID</param>
        void Unsubscribe(ISubscriber subscriber, IEnumerable<T> operates);
        /// <summary>
        ///     发送事件
        /// </summary>
        /// <param name="operate">要发送的事件ID</param>
        /// <param name="args">事件参数</param>
        void SendEvent(T operate, object args);
    }

    /// <summary>
    ///     订阅者
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        ///     接受到消息
        /// </summary>
        /// <param name="subEvent"></param>
        void OnMessage(Event subEvent);
    }
}