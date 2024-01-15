using UnityEditor;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    /// <summary>
    ///     基础的编辑器代码，目前会检测需要展示在Inspector上的方法
    /// </summary>
    [CustomEditor(typeof(object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class BaseZLCEditor : Editor
    {
        public override sealed VisualElement CreateInspectorGUI()
        {
            var root = CreateGUI();
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Constant.ZLC_EDITOR_USS));
            // -- header end --
            return root;
        }

        /// <summary>
        /// 注意：不能继承CreateInspectorGUI
        /// </summary>
        /// <returns></returns>
        protected virtual VisualElement CreateGUI()
        {
            var root = ZLCDrawerHelper.CreateEditorGUI(serializedObject);
            return root;
        }


    }
}