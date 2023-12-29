namespace ZLCEngine.Interfaces
{
    /// <summary>
    /// 存档类型
    /// </summary>
    public enum SaveType
    {
        /// <summary>
        /// 设置类数据
        /// </summary>
        Setting,
        /// <summary>
        /// 较少用到的数据，存数据库中
        /// </summary>
        SQL,
        /// <summary>
        /// 一般游戏存档数据
        /// </summary>
        Model
    }
    
    /// <summary>
    /// 存档器
    /// </summary>
    public interface ISaver : IManager
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="value"></param>
        /// <param name="path">Setting:path当作key；SQL:path当作key Model:path为保存文件的路径</param>
        /// <param name="saveType">保存类型</param>
        /// <typeparam name="T"></typeparam>
        void Save<T>(T value , string path, SaveType saveType = SaveType.Model);
        T Load<T>(string path, SaveType saveType = SaveType.Model);
        int LoadInt(string path, SaveType saveType = SaveType.Setting);
        float LoadFloat(string path, SaveType saveType = SaveType.Setting);
        string LoadString(string path, SaveType saveType = SaveType.Setting);
    }
}