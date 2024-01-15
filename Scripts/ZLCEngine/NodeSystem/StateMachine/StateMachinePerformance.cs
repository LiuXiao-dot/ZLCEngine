using System;
using System.Collections.Generic;
using ZLCEngine.CacheSystem;
using ZLCEngine.Exceptions;
namespace ZLCEngine.NodeSystem.StateMachine
{
    /// <summary>
    /// 状态机用到的池等提高性能的东西
    /// </summary>
    public sealed class StateMachinePerformance : IDisposable
    {
        private ObjectPool<Queue<IGraphNode>> _graphPool;

        private static StateMachinePerformance _instance;

        private StateMachinePerformance()
        {
        }

        public static Queue<IGraphNode> GetQueue()
        {
            if (_instance == null) {
                throw new AlreadyDestroyedException(typeof(StateMachinePerformance));
            }
            if (_instance._graphPool == null) {
                _instance._graphPool = new ObjectPool<Queue<IGraphNode>>(() => new Queue<IGraphNode>(4));
            }
            return _instance._graphPool.Get();
        }

        public static void ReleaseQueue(Queue<IGraphNode> route)
        {
            if (_instance == null) {
                throw new AlreadyDestroyedException(typeof(StateMachinePerformance));
            }
            _instance._graphPool.Release(route);
        }

        internal static void Create()
        {
            if (_instance != null)
                _instance = new StateMachinePerformance();
        }

        internal static void Destroy()
        {
            if (_instance != null) {
                _instance.Dispose();
                _instance = null;
            }
        }

        ~StateMachinePerformance()
        {
            Dispose();
        }

        public void Dispose()
        {
            _graphPool?.Dispose();
        }
    }
}