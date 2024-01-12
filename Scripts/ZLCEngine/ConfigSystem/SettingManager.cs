using UnityEngine;
using ZLCEngine.Interfaces;
namespace ZLCEngine.ConfigSystem
{
    /// <summary>
    /// 设置管理器
    /// </summary>
    public class SettingManager : IManager
    {
        public string GetString()
        {
            return string.Empty;
        }

        public int GetInt()
        {
            return 0;
        }

        public bool GetBool()
        {
            return false;
        }

        public float GetFloat()
        {
            return 0f;
        }
        
        ~SettingManager()
        {
            Dispose();
        }
        public void Dispose()
        {

        }
        public void Init()
        {
            Application.targetFrameRate = 60;
        }
    }
}