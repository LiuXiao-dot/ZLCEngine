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
        public override VisualElement CreateInspectorGUI()
        {
            if (GetType() != typeof(BaseZLCEditor)) return null;
            var root = ZLCDrawerHelper.CreateEditorGUI(serializedObject);
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Constant.ZLC_EDITOR_USS));
            // -- header end --
            return root;
        }


    }
}