using System;
using System.Collections.Generic;
namespace ZLCEngine.CacheSystem
{

    /// <summary>
    ///     对象池
    /// </summary>
    /// <typeparam name="T">池对象的类型</typeparam>
    public interface IObjectPool<T> where T : class
    {
        /// <summary>
        ///     池中剩余的未激活对象数量
        /// </summary>
        int CountInactive { get; }
        /// <summary>
        ///     从池中获取对象
        /// </summary>
        /// <returns>池中的对象</returns>
        T Get();
        /// <summary>
        ///     获取池对象引用
        /// </summary>
        /// <param name="v">池对象</param>
        /// <returns>池对象引用</returns>
        PooledObject<T> Get(out T v);
        /// <summary>
        ///     释放对象到池中
        /// </summary>
        /// <param name="element">待释放的池对象</param>
        void Release(T element);
        /// <summary>
        ///     清空池
        /// </summary>
        void Clear();
    }
    /// <summary>
    ///     池化对象
    /// </summary>
    /// <typeparam name="T">池化对象的实际类型</typeparam>
    public struct PooledObject<T> : IDisposable where T : class
    {
        /// <summary>
        ///     实际对象
        /// </summary>
        private readonly T m_ToReturn;
        /// <summary>
        ///     池
        /// </summary>
        private readonly IObjectPool<T> m_Pool;
        /// <summary>
        ///     池化对象
        /// </summary>
        /// <param name="value">实际对象</param>
        /// <param name="pool">池</param>
        public PooledObject(T value, IObjectPool<T> pool)
        {
            m_ToReturn = value;
            m_Pool = pool;
        }
        /// <summary>
        ///     释放实际对象回池
        /// </summary>
        void IDisposable.Dispose()
        {
            m_Pool.Release(m_ToReturn);
        }
    }
    /// <summary>
    ///     基于栈的对象池
    /// </summary>
    /// <inheritdoc cref="IObjectPool{T}" />
    public class ObjectPool<T> : IDisposable, IObjectPool<T> where T : class
    {
        private readonly Action<T> m_ActionOnDestroy;
        private readonly Action<T> m_ActionOnGet;
        private readonly Action<T> m_ActionOnRelease;
        private readonly Func<T> m_CreateFunc;
        private readonly int m_MaxSize;
        internal readonly Stack<T> m_Stack;
        internal bool m_CollectionCheck;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="createFunc">创建新对象的方法</param>
        /// <param name="actionOnGet">获取对象时调用</param>
        /// <param name="actionOnRelease">释放对象时调用</param>
        /// <param name="actionOnDestroy">销毁对象时调用</param>
        /// <param name="collectionCheck">是否检测释放的对象在池中</param>
        /// <param name="defaultCapacity">默认的池容量</param>
        /// <param name="maxSize">池的最大大小</param>
        /// <exception cref="ArgumentNullException">无创建新对象的方法</exception>
        /// <exception cref="ArgumentException">最大池大小不能小于0</exception>
        public ObjectPool(
            Func<T> createFunc,
            Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000)
        {
            if (createFunc == null)
                throw new ArgumentNullException(nameof(createFunc));
            if (maxSize <= 0)
                throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));
            m_Stack = new Stack<T>(defaultCapacity);
            m_CreateFunc = createFunc;
            m_MaxSize = maxSize;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            m_ActionOnDestroy = actionOnDestroy;
            m_CollectionCheck = collectionCheck;
        }

        /// <summary>
        ///     池中创建过的全部对象的数量
        /// </summary>
        public int CountAll { get; private set; }

        /// <summary>
        ///     池中被激活的对象的数量
        /// </summary>
        public int CountActive
        {
            get {
                return CountAll - CountInactive;
            }
        }
        /// <inheritdoc />
        public void Dispose()
        {
            Clear();
        }

        /// <summary>
        ///     池中未被激活的对象的数量
        /// </summary>
        public int CountInactive
        {
            get {
                return m_Stack.Count;
            }
        }

        /// <inheritdoc />
        public T Get()
        {
            T obj;
            if (m_Stack.Count == 0) {
                obj = m_CreateFunc();
                ++CountAll;
            } else
                obj = m_Stack.Pop();
            Action<T> actionOnGet = m_ActionOnGet;
            if (actionOnGet != null)
                actionOnGet(obj);
            return obj;
        }

        /// <inheritdoc />
        public PooledObject<T> Get(out T v)
        {
            return new PooledObject<T>(v = Get(), this);
        }

        /// <inheritdoc />
        public void Release(T element)
        {
            if (m_CollectionCheck && m_Stack.Count > 0 && m_Stack.Contains(element))
                throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
            Action<T> actionOnRelease = m_ActionOnRelease;
            if (actionOnRelease != null)
                actionOnRelease(element);
            if (CountInactive < m_MaxSize) {
                m_Stack.Push(element);
            } else {
                Action<T> actionOnDestroy = m_ActionOnDestroy;
                if (actionOnDestroy != null)
                    actionOnDestroy(element);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (m_ActionOnDestroy != null) {
                foreach (T obj in m_Stack)
                    m_ActionOnDestroy(obj);
            }
            m_Stack.Clear();
            CountAll = 0;
        }

        /// <summary>
        ///     析构函数
        /// </summary>
        ~ObjectPool()
        {
            Dispose();
        }
    }
}