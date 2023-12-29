using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using ZLCEngine.UGUISystem;
namespace ZLCEditor.UGUISystem
{
    [CustomEditor(typeof(BlitImage), true)]
    [CanEditMultipleObjects]
    public class BlitImageEditor : ImageEditor
    {
        private SerializedProperty _blitSprite;
        private SerializedProperty _type;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _blitSprite = serializedObject.FindProperty("_blitSprite");
            _type = serializedObject.FindProperty("m_Type");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_blitSprite);
            if (EditorGUI.EndChangeCheck())
            {
                var newSprite = _blitSprite.objectReferenceValue as Sprite;
                if (newSprite)
                {
                    Image.Type oldType = (Image.Type)_type.enumValueIndex;
                    if (newSprite.border.SqrMagnitude() > 0)
                    {
                        _type.enumValueIndex = (int)Image.Type.Sliced;
                    }
                    else if (oldType == Image.Type.Sliced)
                    {
                        _type.enumValueIndex = (int)Image.Type.Simple;
                    }
                }
                (serializedObject.targetObject as Image).DisableSpriteOptimizations();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}