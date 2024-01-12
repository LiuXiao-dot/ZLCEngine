using System;
using UnityEngine;
using ZLCEngine.SerializeTypes;
namespace ZLCEngine.UGUISystem
{
    /// <summary>
    ///     ZLCEngine中的UI样式
    /// </summary>
    [Serializable]
    public class ZUI : MonoBehaviour, ISerializationCallbackReceiver
    {
        public AZUI ui;
        /// <summary>
        ///     AZUI的序列化信息
        /// </summary>
        [SerializeField] [HideInInspector] private string uiCache;
        [SerializeField] [HideInInspector] private SType uiType;

        public void OnBeforeSerialize()
        {
            if (ui == null) {
                uiCache = null;
                return;
            }
            // todo:使用自定义序列化工具
            uiType = ui.GetType();
            uiCache = JsonUtility.ToJson(ui);
        }
        public void OnAfterDeserialize()
        {
            /*if (uiCache != null && uiCache.Length > 0) {
                ui = (AZUI)JsonUtility.FromJson(uiCache,uiType);
            }*/
        }
    }

}