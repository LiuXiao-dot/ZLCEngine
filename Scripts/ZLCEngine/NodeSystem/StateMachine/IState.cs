namespace ZLCEngine.NodeSystem.StateMachine
{
    /// <summary>
    /// 状态机的状态
    /// </summary>
    public interface IState : IGraphNode
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        /// <returns>false:进入状态失败</returns>
        bool Enter(IContext context);
        /// <summary>
        /// 退出状态
        /// </summary>
        /// <returns>false:退出状态失败</returns>
        bool Exit(IContext context);

        /// <summary>
        /// 是否可以进入状态
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="oldState">上一个状态</param>
        /// <returns></returns>
        bool CanEnter(IContext context, IState oldState = null);

        /// <summary>
        /// 更新状态
        /// </summary>
        void Update(IContext context);
    }
}