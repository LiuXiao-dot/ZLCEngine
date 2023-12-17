using UnityEditor;
using UnityEngine;
using ZLCEngine.Interfaces;
namespace ZLCEngine.ConfigSystem
{
    /// <summary>
    ///     单例ScriptableObject
    /// </summary>
    public abstract class SOSingleton<T> : ScriptableObject where T : SOSingleton<T>
    {
        private static bool isPlaying;
        /// <summary>
        ///     SO单例
        /// </summary>
        private static T _instance;
        /// <summary>
        ///     SO单例
        /// </summary>
        public static T Instance
        {
            get {
                if (_instance != null) return _instance;
                if (isPlaying) {
                    // 运行中，使用加载器加载资源
                    IAppLauncher.Get<IResLoader>().LoadAssetSync($"{typeof(T).Name}.asset", out _instance);
                } else {
                #if UNITY_EDITOR
                    CheckAsset();
                #else
                    Debug.LogError($"缺少{typeof(T).FullName}资产");
                #endif
                }
                return _instance;
            }
        }

        /// <summary>
        ///     激活时为<see cref="Instance" />赋值.
        /// </summary>
        protected virtual void OnEnable()
        {
            isPlaying = Application.isPlaying;
            if (_instance != null) {
                if (this != _instance) {
                    Debug.LogWarning($"存在多个单例SO的实例{typeof(T).FullName}");
                    return;
                }
                return;
            }

            _instance = (T)this;
            if (FilePathAttribute.GetPath(typeof(T)) != null && _instance.name != typeof(T).Name) {
                Debug.LogWarning($"单例{_instance.name}名称建议是:{typeof(T).Name},可以自动加载，如果是自定义名称,需要手动提前加载才能正常使用.");
            }
        }

        /// <summary>
        ///     销毁时，如果被销毁的是单例，则将单例置空
        /// </summary>
        protected virtual void OnDisable()
        {
            if (this == _instance) _instance = null;
        }

        #if UNITY_EDITOR
        /// <summary>
        ///     检测是否存在资源
        /// </summary>
        private static void CheckAsset()
        {
            if (_instance != null) return;
            string path = FilePathAttribute.GetPath(typeof(T));
            if (path == string.Empty) {
                // 不缓存
                T temp = CreateInstance<T>();
                temp.hideFlags = HideFlags.DontSave;
                temp.name = typeof(T).Name;
                _instance = temp;
            } else {
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                    _instance = asset;
                else {
                    T temp = CreateInstance<T>();
                    AssetDatabase.CreateAsset(temp, path);
                    _instance = temp;
                }
            }
        }
        #endif
    }
}