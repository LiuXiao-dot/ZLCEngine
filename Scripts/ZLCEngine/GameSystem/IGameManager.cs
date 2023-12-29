using System;
using ZLCEngine.Interfaces;
namespace ZLCEngine.GameSystem
{
    /// <summary>
    /// 连连看玩法管理器
    /// </summary>
    public interface IGameManager : IDisposable, ILoader
    {
        /// <summary>
        /// 创造一场游戏
        /// </summary>
        /// <param name="config">配置信息，用于初始化游戏，以及决定游戏的类型等基础内容</param>
        /// <param name="model">游戏数据，有游戏数据时将视为加载存档，将从model中还原游戏</param>
        /// <returns></returns>
        IGame Create(IGameConfig config, IGameModel model = null);

        /// <summary>
        /// 退出游戏
        /// </summary>
        void Exit();
    }
}