namespace ZLCEngine.WindowSystem
{
    /// <summary>
    /// 窗口层级
    /// </summary>
    public enum WindowLayer
    {
#region CoreWindow
        MAIN,
        CHILD,
        SMALL,
#endregion
#region ExtraWindow
        POPUP,
        PANEL,
#endregion
#region IgnoreWindow
        LOADING,
        MASK,
        TIP,
#endregion
    }
}