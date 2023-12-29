using System;
using System.IO;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;
using ZLCEngine.Interfaces;
using ZLCEngine.SerializeTypes;
using ZLCEngine.Utils;
namespace ZLCEngine.SaveSystem
{
    public class Saver : ISaver
    {
        private static string BasePath => Application.persistentDataPath;

        public void Init()
        {
            var types = CustomResolverSO.Instance.resolveTypes;
            var resolves = new IFormatterResolver[2 + (types == null ? 0 : types.Length)];
            resolves[0] = MessagePack.Unity.Extension.UnityBlitResolver.Instance;
            resolves[1] = MessagePack.Unity.UnityResolver.Instance;
            if (types != null) {
                for (int i = 0; i < types.Length; i++) {
                    resolves[i + 2] = types[i].realType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(null) as IFormatterResolver;
                }
            }
            var resolver = CompositeResolver.Create(resolves);
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

            MessagePackSerializer.DefaultOptions = options;
        }

        /// <inheritdoc/>
        public void Save<T>(T value, string path, SaveType saveType = SaveType.Model)
        {
            switch (saveType) {
                case SaveType.Setting:
                    switch (value) {
                        case string sValue:
                            PlayerPrefs.SetString(path, sValue);
                            break;
                        case int iValue:
                            PlayerPrefs.SetInt(path, iValue);
                            break;
                        case float fValue:
                            PlayerPrefs.SetFloat(path, fValue);
                            break;
                    }
                    #if UNITY_EDITOR
                    SaveEditor(path, value.ToString());
                    #endif
                    break;
                case SaveType.SQL:
                    break;
                case SaveType.Model:
                    var fullPath = Path.Combine(BasePath, path);
                    var bytes = MessagePackSerializer.Serialize(value);
                    #if UNITY_EDITOR
                    Debug.Log(fullPath);
                    SaveEditor(fullPath, MessagePackSerializer.ConvertToJson(bytes));
                    #endif
                    FileHelper.CheckDirectory(Path.GetDirectoryName(fullPath));
                    File.WriteAllBytes(Path.Combine(BasePath, path), bytes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }
        }

        /// <inheritdoc/>
        public T Load<T>(string path, SaveType saveType = SaveType.Model)
        {
            switch (saveType) {
                case SaveType.SQL:
                    break;
                case SaveType.Model:
                    var bytes = File.ReadAllBytes(Path.Combine(BasePath, path));
                    return MessagePackSerializer.Deserialize<T>(bytes);
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }
            return default(T);
        }

        private bool CheckKey(string key)
        {
            if (!PlayerPrefs.HasKey(key)) {
                Debug.LogWarning($"未设置{key}的值");
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public int LoadInt(string path, SaveType saveType = SaveType.Setting)
        {
            if (CheckKey(path))
                return PlayerPrefs.GetInt(path);
            return -1;
        }

        /// <inheritdoc/>
        public float LoadFloat(string path, SaveType saveType = SaveType.Setting)
        {
            if (CheckKey(path))
                return PlayerPrefs.GetFloat(path);
            return -1;
        }

        /// <inheritdoc/>
        public string LoadString(string path, SaveType saveType = SaveType.Setting)
        {
            if (CheckKey(path))
                return PlayerPrefs.GetString(path);
            return string.Empty;
        }

        #if UNITY_EDITOR
        private void SaveEditor(string key, string value)
        {
            CustomResolverSO.Instance.saved ??= new SDictionary<string, string>();
            var saved = CustomResolverSO.Instance.saved;
            if (saved.ContainsKey(key)) {
                saved[key] = value;
            } else {
                saved.Add(key, value);
            }
        }
        #endif

        ~Saver()
        {
            Dispose();
        }
        public void Dispose()
        {

        }
    }
}