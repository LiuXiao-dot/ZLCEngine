using System;
using System.Collections.Generic;
using ZLCEngine.Exceptions;
namespace ZLCEngine.NodeSystem.StateMachine
{
    /// <summary>
    /// 状态机
    /// </summary>
    public class StateMachine : IDisposable
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        private IState _currentState;

        /// <summary>
        /// 状态上下文
        /// </summary>
        private IContext _context;

        /// <summary>
        /// 状态机的树
        /// 1.状态节点为IState,
        /// 2.判断节点为IDecision
        /// 3.如果两个IState之间没有任何节点，将只能靠手动调用ChangeState切换到目标状态
        /// 4.如果两个IState之间有IDecision节点，将判断IDecision节点是否成立，IDecision节点只能有一个子节点，但是可以有多个父节点
        /// 5.两个IState之间最多只有一个IDecision节点
        /// </summary>
        private Graph _graph;

        /// <summary>
        /// 路径
        /// </summary>
        private List<IGraphNode> _decisions;

        public StateMachine()
        {
            StateMachinePerformance.Create();
            _decisions = new List<IGraphNode>();
        }

        /// <summary>
        /// 切换状态
        /// 1.每次切换状态之后都会获取可能的状态切换路径
        /// </summary>
        /// <returns>true:状态切换成功 false:状态切换失败</returns>
        public bool ChangeState(IState newState)
        {
            try {
                if (!newState.CanEnter(_context, _currentState)) return false;
                if (_currentState != null && !_currentState.Exit(_context)) return false;
                if (newState.Enter(_context)) {
                    _decisions.Clear();

                    var neighbours = newState.neighbours;
                    foreach (var neighbour in neighbours) {
                        // 不存在IDecision节点时，不需要每次Update判断是否要切换状态，不用添加到队列中
                        if (neighbour is IDecision) {
                            _decisions.Add(neighbour);
                        }
                    }
                    
                    return true;
                }

                throw new UnexpectedException("newState.CanEnter(_context, _currentState)==true", "newState.Enter(_context)返回true", "newState.Enter(_context)返回了false");
            }
            catch (ZLCException e) {
                // 切换失败，直接不改变状态
                e.Handle();
                return false;
            }
        }

        /// <summary>
        /// 状态切换检测
        /// </summary>
        private void UpdateStateChangeDetection()
        {
            foreach (var decision in _decisions) {
                if (!(decision as IDecision).Detection(_currentState, out var toState)) continue;
                if (ChangeState(toState)) return;
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public void Update()
        {
            if (_currentState == null) return;

            try {
                //状态成功切换后会立刻执行一次Update
                UpdateStateChangeDetection();
                _currentState.Update(_context);
            }
            catch (ZLCException e) {
                // 出错后，根据错误类型进行处理,默认直接调用对应错误的Handle方法，不进行其他处理
                // todo:其他错误类型的处理
                e.Handle();
            }
        }

        public void Dispose()
        {
            try {
                if (_decisions != null) {
                    _decisions.Clear();
                }
            }
            catch (ZLCException e) {
                e.Handle();
            }
            finally {
                StateMachinePerformance.Destroy();
            }
        }
    }
}