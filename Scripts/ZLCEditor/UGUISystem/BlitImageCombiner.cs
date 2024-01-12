using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.UGUISystem
{
    /// <summary>
    /// 将两张图片组合起来
    /// </summary>
    [Tool("工具/图片组合")]
    [FilePath(FilePathAttribute.PathType.XWEditor, true)]
    public class BlitImageCombiner : SOSingleton<BlitImageCombiner>
    {
        public Sprite source1;
        public Sprite source2;
        public Sprite result;
        public string targetPath;

        [Button("合成图片")]
        private void Generate()
        {
            var source1 = this.source1.texture;
            var source2 = this.source2.texture;
            
            // 合成
            var width = source1.width;
            var height = source1.height;

            var resultTex = new Texture2D(width, height);
                
            for (int j = 0; j < width; j++) {
                for (int k = 0; k < height; k++) {
                    var alpha = 1 - source2.GetPixel(j, k).r;
                    var color = source1.GetPixel(j, k);
                    color.a = alpha;
                    resultTex.SetPixel(j,k, color);
                }
            }
            resultTex.Apply();
            var resultPath = $"{targetPath}/{source1.name}.png";
            File.WriteAllBytes(resultPath, resultTex.EncodeToPNG());
            AssetDatabase.Refresh();
            TextureImporter smallTextureImp = AssetImporter.GetAtPath(resultPath) as TextureImporter;
            smallTextureImp.isReadable = true;
            smallTextureImp.alphaIsTransparency = true;
            // 不开启mipmap
            smallTextureImp.mipmapEnabled = false;
            AssetDatabase.ImportAsset(resultPath);
        }
        
        [Button("批量合成图片")]
        private void Excute()
        {
            var sourcePath = "Assets/Arts/Textures/Game/element_bg";
            var targetPath = "Assets/Arts/Textures/Game/element_bg_combined";
            /*var urls = AssetDatabase.FindAssets("t:Texture2D", new[]
            {
                sourcePath
            });*/
            var length = 7;
            Texture2D resultTex;
            for (int i = 1; i < length; i++) {
                var path1 = $"{sourcePath}/element_bg_{i}.png";
                var path2 = $"{sourcePath}/element_bg_0.png";
                var source1 = AssetDatabase.LoadAssetAtPath<Texture2D>(path1);
                var source2 = AssetDatabase.LoadAssetAtPath<Texture2D>(path2);
                
                // 合成
                var width = source1.width;
                var height = source1.height;

                resultTex = new Texture2D(width, height);
                
                for (int j = 0; j < width; j++) {
                    for (int k = 0; k < height; k++) {
                        var alpha = 1 - source2.GetPixel(j, k).r;
                        var color = source1.GetPixel(j, k);
                        color.a = alpha;
                        resultTex.SetPixel(j,k, color);
                    }
                }
                resultTex.Apply();
                var resultPath = $"{targetPath}/element_bg_{i - 1}.png";
                File.WriteAllBytes(resultPath, resultTex.EncodeToPNG());
                AssetDatabase.Refresh();
                TextureImporter smallTextureImp = AssetImporter.GetAtPath(resultPath) as TextureImporter;
                smallTextureImp.isReadable = true;
                smallTextureImp.alphaIsTransparency = true;
                // 不开启mipmap
                smallTextureImp.mipmapEnabled = false;
                AssetDatabase.ImportAsset(resultPath);
            }
        }
    }

    [CustomEditor(typeof(BlitImageCombiner))]
    public class BlitIamgeCombinerEditor : BaseZLCEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var source1 = serializedObject.FindProperty("source1");
            var source2 = serializedObject.FindProperty("source2");
            var result = serializedObject.FindProperty("result");
            var targetPath = serializedObject.FindProperty("targetPath");
            root.Add(new PropertyField(targetPath));
            AddSprite(root,source1);
            AddSprite(root,source2);
            AddSprite(root,result);
            root.Add(base.CreateInspectorGUI());
            return root;
        }

        private void AddSprite(VisualElement parent, SerializedProperty spriteProperty)
        {
            var ve = new VisualElement();
            var sprite = (Sprite)spriteProperty.objectReferenceValue;
            var property = new PropertyField(spriteProperty);
            property.RegisterValueChangeCallback(e =>
            {
                sprite = (Sprite)e.changedProperty.objectReferenceValue;
                if (sprite != null) {
                    ve.style.minWidth = 100 * sprite.rect.height / sprite.rect.width;
                    ve.style.minHeight = 100;
                    ve.style.maxWidth = 300 * sprite.rect.height / sprite.rect.width ;
                    ve.style.maxHeight = 300 ;
                    ve.style.backgroundImage = new StyleBackground(sprite);
                }
            });
            parent.Add(property);
            if (sprite != null) {
                var spriteParent = new VisualElement(); // 用于居中图片
                ve.style.maxWidth = sprite.rect.width ;
                ve.style.maxHeight = sprite.rect.height ;
                ve.style.backgroundImage = new StyleBackground(sprite);
                spriteParent.Add(ve);
                parent.Add(spriteParent);
            }
        }
    }
}