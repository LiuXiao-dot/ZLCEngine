﻿#if UNITY_EDITOR

using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;

namespace MessagePack.Unity.Editor
{
    [Tool("存档/配置")]
    [FilePath(FilePathAttribute.PathType.XWEditor, true)]
    [Serializable]
    internal class MessagePackWindow : SOSingleton<MessagePackWindow>
    {
        static MessagePackWindow window;

        [SerializeField]public bool processInitialized;
        [SerializeField]public bool isDotnetInstalled;
        [SerializeField]public string dotnetVersion;
        [SerializeField]public bool isInstalledMpc;
        [SerializeField]public bool installingMpc;
        [SerializeField]public bool invokingMpc;

        [SerializeField]public MpcArgument mpcArgument;

        protected override async void OnEnable()
        {
            base.OnEnable();
            window = this; // set singleton.
            try
            {
                var dotnet = await ProcessHelper.FindDotnetAsync();
                isDotnetInstalled = dotnet.found;
                dotnetVersion = dotnet.version;

                if (isDotnetInstalled)
                {
                    isInstalledMpc = await ProcessHelper.IsInstalledMpc();
                }
            }
            finally
            {
                mpcArgument = MpcArgument.Restore();
                processInitialized = true;
            }
        }
    }

    [Serializable]
    internal class MpcArgument
    {
        [SerializeField]public string Input;
        [SerializeField]public string Output;
        [SerializeField]public string ConditionalSymbol;
        [SerializeField]public string ResolverName;
        [SerializeField]public string Namespace;
        [SerializeField]public bool UseMapMode;
        [SerializeField]public string MultipleIfDirectiveOutputSymbols;

        static string Key => "MessagePackCodeGen." + Application.productName;

        public static MpcArgument Restore()
        {
            if (EditorPrefs.HasKey(Key))
            {
                var json = EditorPrefs.GetString(Key);
                return JsonUtility.FromJson<MpcArgument>(json);
            }
            else
            {
                return new MpcArgument();
            }
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(this);
            EditorPrefs.SetString(Key, json);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("-i "); sb.Append(Input);
            sb.Append(" -o "); sb.Append(Output);
            if (!string.IsNullOrWhiteSpace(ConditionalSymbol))
            {
                sb.Append(" -c "); sb.Append(ConditionalSymbol);
            }
            if (!string.IsNullOrWhiteSpace(ResolverName))
            {
                sb.Append(" -r "); sb.Append(ResolverName);
            }
            if (UseMapMode)
            {
                sb.Append(" -m");
            }
            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                sb.Append(" -n "); sb.Append(Namespace);
            }
            if (!string.IsNullOrWhiteSpace(MultipleIfDirectiveOutputSymbols))
            {
                sb.Append(" -ms "); sb.Append(MultipleIfDirectiveOutputSymbols);
            }

            return sb.ToString();
        }
    }

    internal static class ProcessHelper
    {
        const string InstallName = "messagepack.generator";

        public static async Task<bool> IsInstalledMpc()
        {
            var list = await InvokeProcessStartAsync("dotnet", "tool list -g");
            if (list.Contains(InstallName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<string> InstallMpc()
        {
            return await InvokeProcessStartAsync("dotnet", "tool install --global " + InstallName);
        }

        public static async Task<(bool found, string version)> FindDotnetAsync()
        {
            try
            {
                var version = await InvokeProcessStartAsync("dotnet", "--version");
                return (true, version);
            }
            catch
            {
                return (false, null);
            }
        }

        public static Task<string> InvokeProcessStartAsync(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = Application.dataPath
            };

            Process p;
            try
            {
                p = Process.Start(psi);
            }
            catch (Exception ex)
            {
                return Task.FromException<string>(ex);
            }

            var tcs = new TaskCompletionSource<string>();
            p.EnableRaisingEvents = true;
            p.Exited += (object sender, System.EventArgs e) =>
            {
                var data = p.StandardOutput.ReadToEnd();
                p.Dispose();
                p = null;

                tcs.TrySetResult(data);
            };

            return tcs.Task;
        }
    }
}

#endif
