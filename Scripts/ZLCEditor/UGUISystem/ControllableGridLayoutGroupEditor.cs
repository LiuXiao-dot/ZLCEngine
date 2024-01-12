#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
using ZLCEngine.UGUISystem;
namespace ZLCEditor.UGUISystem
{
    [CustomEditor(typeof(ControllableGridLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class ControllableGridLayoutGroupEditor : GridLayoutGroupEditor
    {
        SerializedProperty m_OffsetX;
        SerializedProperty m_OffsetY;
        SerializedProperty m_View;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_OffsetX = serializedObject.FindProperty("offsetX");
            m_OffsetY = serializedObject.FindProperty("offsetY");
            m_View = serializedObject.FindProperty("view");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(m_OffsetX, true);
            EditorGUILayout.PropertyField(m_OffsetY, true);
            EditorGUILayout.PropertyField(m_View, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif