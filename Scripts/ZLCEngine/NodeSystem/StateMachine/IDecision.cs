namespace ZLCEngine.NodeSystem.StateMachine
{
    /// <summary>
    /// 判断
    /// 在两个IState之间使用，用于判断是否要从一个IState切换到另一个IState
    /// 状态机将自动进行检测与状态切换
    ///
    /// Parent:原本的状态
    /// Children:要切换到的状态列表
    ///
    /// IDecision只能有两个邻居
    /// </summary>
    public interface IDecision : IGraphNode
    {
        /// <summary>
        /// 检测是否要切换
        /// </summary>
        /// <param name="fromState">来源状态</param>
        /// <param name="toState">将切换到的状态</param>
        /// <returns>true:可以切换</returns>
        bool Detection(IState fromState, out IState toState);

        /// <summary>
        /// 传入IDecision包含的其中一个状态，返回另一个状态
        /// </summary>
        /// <returns></returns>
        IState GetAnother(IState aState);
    }
}