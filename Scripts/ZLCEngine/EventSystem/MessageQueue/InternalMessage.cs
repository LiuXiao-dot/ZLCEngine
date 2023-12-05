namespace ZLCEngine.EventSystem.MessageQueue
{
    /// <summary>
    /// 场景消息
    /// </summary>
    public enum SceneMessage
    {
        OnSceneOpen,
        OnSceneClose,   
    }
    
    /// <summary>
    /// 窗口消息
    /// </summary>
    public enum WindowMessage
    {
        #region 窗口相关
        /// <summary>
        /// 窗口资源加载完成
        /// </summary>
        AfterWindowLoaded,
        /// <summary>
        /// 窗口资源加载完成->窗口进入前
        /// </summary>
        BeforeWindowEnter,
        /// <summary>
        /// 窗口进入前->窗口进入后
        /// </summary>
        AfterWindowEnter,
        /// <summary>
        /// 窗口恢复前
        /// </summary>
        BeforeWindowResume,
        /// <summary>
        /// 窗口恢复
        /// </summary>
        AfterWindowResume,
        /// <summary>
        /// 窗口暂停前
        /// </summary>
        BeforeWindowPause,
        /// <summary>
        /// 窗口暂停
        /// </summary>
        AfterWindowPause,
        /// <summary>
        /// 窗口退出前
        /// </summary>
        BeforeWindowExit,
        /// <summary>
        /// 窗口退出
        /// </summary>
        AfterWindowExit,
        #endregion
    }
}