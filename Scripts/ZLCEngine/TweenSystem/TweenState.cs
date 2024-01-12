namespace ZLCEngine.TweenSystem
{
    public enum TweenState
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        UnInit,
        /// <summary>
        /// 已初始化后的闲置状态
        /// </summary>
        Idle,
        /// <summary>
        /// 播放中
        /// </summary>
        Playing,
        /// <summary>
        /// 暂停中
        /// </summary>
        Pausing,
        /// <summary>
        /// 动画状态错误，无法再使用
        /// </summary>
        Error
    }
}