using System.Collections.Generic;
namespace ZLCEngine.NodeSystem.StateMachine.Basic
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class AState : IState
    {
        public List<IGraphNode> neighbours { get; set; }
        
        public virtual bool Enter(IContext context)
        {
            return true;
        }
        
        public virtual bool Exit(IContext context)
        {
            return true;
        }
        
        public virtual bool CanEnter(IContext context, IState oldState = null)
        {
            return true;
        }
        
        public abstract void Update(IContext context);
    }
}