namespace ZLCEngine.TweenSystem
{
    /// <summary>
    /// 补间动画接口
    /// </summary>
    public interface ITween
    {
        void Init();
        /// <summary>
        /// 动画的状态
        /// </summary>
        TweenState TweenState { get; }
        /// <summary>
        /// 开始播放动画
        /// </summary>
        /// <param name="speed">speed影响动画播放速度，负数表示倒放</param>
        /// <param name="startTime">开始播放的时间点</param>
        /// <param name="forcePlay">true:如果动画正在播放，将强制重新开始 false:如果动画正在播放，将不会再执行播放逻辑</param>
        void Play(float speed = 1,float startTime = 0, bool forcePlay = false);
        /// <summary>
        /// 暂停动画
        /// </summary>
        void Pause();
        /// <summary>
        /// 恢复被暂停的动画，若没有被暂停，将无效
        /// </summary>
        void Resume();
        /// <summary>
        /// 结束动画，将动画直接设定到播放完成，并且会触发播放完成的回调
        /// </summary>
        void Finish();
        /// <summary>
        /// 强制设定为time时间的状态，并修改播放时间为time，若正在播放，则继续，否则不播放动画
        /// </summary>
        /// <param name="time"></param>
        void ForceUpdate(float time = 0);
        /// <summary>
        /// 重置状态
        /// </summary>
        void Reset();
    }
}