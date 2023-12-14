using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
using ZLCEditor.ZLCCoding;
using ZLCEngine.SerializeTypes;
using ZLCEngine.WindowSystem;
namespace ZLCEditor.WindowSystem
{
    public class WindowViewCode
    {
        public string code;
    }

    /// <summary>
    ///     生成的代码，不会查找每个组件，需要直接将各个组件关联到变量上
    /// </summary>
    public class WindowGo2ViewConverter : IFormatConverter<GameObject, WindowViewCode>
    {
        private static string defaultCode =
            $@"using ZLCEngine.WindowSystem;
using ZLCEngine.Inspector;
#for ${ZLCCoding.Constant.Using}$ #
${ZLCCoding.Constant.Using}$
#end
namespace ZLCGenerate.Window
{{
    public class ${ZLCCoding.Constant.ClassName}$ : AWindowView
    {{
#for ${ZLCCoding.Constant.FieldsDEF}$ #
        [ReadOnly]${ZLCCoding.Constant.FieldsDEF}$
#end
        private void Awake()
        {{
#for ${ZLCCoding.Constant.FieldsSET}$ #
            ${ZLCCoding.Constant.FieldsSET}$
#end
        }}
    }}
}}";

        public WindowViewCode Convert(GameObject from)
        {
            CheckComponents(from, out string[] fieldDefs, out string[] fieldSets, out string[] usings);

            ZLCCode zlcCode = new ZLCCode
            {
                code = defaultCode,
                kvs = new SDictionary<string, object>
                {
                    {
                        ZLCCoding.Constant.ClassName, $"{from.name}View"
                    },
                    {
                        ZLCCoding.Constant.FieldsDEF, fieldDefs
                    },
                    {
                        ZLCCoding.Constant.FieldsSET, fieldSets
                    },
                    {
                        ZLCCoding.Constant.Using, usings
                    }
                }
            };
            CSharpCode cSharpCode = FormatManager.Convert<ZLCCode, CSharpCode>(zlcCode);

            return new WindowViewCode
            {
                code = cSharpCode.code
            };
        }

        /// <summary>
        ///     检测全部要生成代码的组件
        /// </summary>
        private void CheckComponents(GameObject from, out string[] fieldDefs, out string[] fieldSets, out string[] usings)
        {
            List<string> fields = new List<string>();
            HashSet<Type> usingHash = new HashSet<Type>();
            WindowComponent[] components = from.GetComponentsInChildren<WindowComponent>();
            foreach (WindowComponent component in components) {
                MonoBehaviour[] temps = component.components;
                string goName = component.gameObject.name;
                foreach (MonoBehaviour temp in temps) {
                    fields.Add($"public {temp.GetType().Name} {goName}_{temp.GetType().Name};");
                    usingHash.Add(temp.GetType());
                }
            }
            fieldDefs = fields.ToArray();
            fieldSets = new string[]
            {
            };
            usings = usingHash.Select(temp => $"using {temp.Namespace};").ToArray();
        }
    }
}