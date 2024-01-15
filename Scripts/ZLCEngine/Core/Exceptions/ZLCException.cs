using System;
namespace ZLCEngine.Exceptions
{
    public abstract class ZLCException : ApplicationException
    {
        protected abstract string GetExceptionMessage();

        /// <summary>
        /// 处理当前异常
        /// </summary>
        public virtual void Handle()
        {
            UnityEngine.Debug.LogError(this);
        }

        public override string ToString()
        {
            return $"<color=0#FF7675>{GetExceptionMessage()}</color>\n{base.ToString()}";
        }
    }
}