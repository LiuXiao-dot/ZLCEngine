using System;
namespace ZLCEngine.Exceptions
{
    /// <summary>
    /// 要调用的对象已经被销毁了
    /// </summary>
    public class AlreadyDestroyedException : ZLCException
    {
        private Type _type;

        public AlreadyDestroyedException(Type type)
        {
            this._type = type;
        }
        
        protected override string GetExceptionMessage()
        {
            return $"{_type.FullName}已经被销毁了,但仍在被调用.";
        }
    }
}