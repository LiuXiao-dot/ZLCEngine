using System.Collections.Generic;
namespace ZLCEngine.NodeSystem.StateMachine.Basic
{
    /// <summary>
    /// Decision基类
    /// </summary>
    public abstract class ADecision : IDecision
    {
        public List<IGraphNode> neighbours { get; set; }
        
        public virtual bool Detection(IState fromState, out IState toState)
        {
            toState = GetAnother(fromState);
            return false;
        }
        
        public IState GetAnother(IState aState)
        {
            if (aState == neighbours[0]) {
                return neighbours[1] as IState;
            }
            return neighbours[0] as IState;
        }
    }
}