using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
namespace ZLCEditor.Tool
{
    /// <summary>
    /// 复合图片拆分
    /// </summary>
    [Tool("工具/复合图片拆分")]
    [Serializable]
    public class MultiTextureSplitTool
    {
        public Texture2D texture;

        [Button]
        public void Excute()
        {
            if (texture == null) return;

            // 获取所选图片
            Texture2D selectedImg = texture;
            
            var assetPath = AssetDatabase.GetAssetPath(selectedImg);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            string rootPath = Path.GetDirectoryName(assetPath);
            TextureImporter texImp = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            // 设置为可读
            texImp.isReadable = true;
            AssetDatabase.ImportAsset(assetPath);

            // 创建文件夹
            FileHelper.CheckDirectory(Path.Combine(rootPath, selectedImg.name));

            foreach (var obj in sprites) {
                if (obj is Sprite sprite) {
                    var targetTex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                    var pixels = sprite.texture.GetPixels(
                        (int)sprite.textureRect.x,
                        (int)sprite.textureRect.y,
                        (int)sprite.textureRect.width,
                        (int)sprite.textureRect.height);
                    targetTex.SetPixels(pixels);
                    targetTex.Apply();
                    
                    // 保存小图文件
                    string smallImgPath = rootPath + "/" + selectedImg.name + "/" + sprite.name + ".png";
                    File.WriteAllBytes(smallImgPath, targetTex.EncodeToPNG());
                    // 刷新资源窗口界面
                    AssetDatabase.Refresh();
                    // 设置小图的格式
                    TextureImporter smallTextureImp = AssetImporter.GetAtPath(smallImgPath) as TextureImporter;
                    // 设置为可读
                    smallTextureImp.isReadable = true;
                    // 设置alpha通道
                    smallTextureImp.alphaIsTransparency = true;
                    // 不开启mipmap
                    smallTextureImp.mipmapEnabled = false;
                    AssetDatabase.ImportAsset(smallImgPath);
                }
            }
        }
    }
}