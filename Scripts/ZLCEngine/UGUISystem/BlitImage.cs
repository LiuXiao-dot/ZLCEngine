using System;
using UnityEngine;
using UnityEngine.UI;
namespace ZLCEngine.UGUISystem
{
    [Serializable]
    public class BlitImage : Image
    {
        public Sprite blitSprite
        {
            get {
                return _blitSprite;
            }
            set {
                isBlitDirty = _blitSprite != value;
                _blitSprite = value;
                RefreshBlitTexture();
            }
        }

        [SerializeField]
        private Sprite _blitSprite;
        
        private bool isBlitDirty;

        public override void SetMaterialDirty()
        {
            base.SetMaterialDirty();
            SetSpriteAtlasUV(sprite, material, "Main");
            RefreshBlitTexture();
        }

        public void RefreshBlitTexture()
        {
            if(!isBlitDirty) return;
            if(material == null) return;
            if (blitSprite != null) {
                //sprite为图集中的某个子Sprite对象
                var targetTex = new Texture2D((int)blitSprite.rect.width, (int)blitSprite.rect.height);
                var pixels = blitSprite.texture.GetPixels(
                    (int)blitSprite.textureRect.x,
                    (int)blitSprite.textureRect.y,
                    (int)blitSprite.textureRect.width,
                    (int)blitSprite.textureRect.height);
                targetTex.SetPixels(pixels);
                targetTex.Apply();

                material.SetTexture( "_BlitMap",targetTex);
            } else {
                material.SetTexture( "_BlitMap",null);
            }
            isBlitDirty = true;
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            // 设定材质中的_BlitMap纹理
            if(material == null) return;
            if (blitSprite != null) {
                //sprite为图集中的某个子Sprite对象
                var targetTex = new Texture2D((int)blitSprite.rect.width, (int)blitSprite.rect.height);
                var pixels = blitSprite.texture.GetPixels(
                    (int)blitSprite.textureRect.x,
                    (int)blitSprite.textureRect.y,
                    (int)blitSprite.textureRect.width,
                    (int)blitSprite.textureRect.height);
                targetTex.SetPixels(pixels);
                targetTex.Apply();

                material.SetTexture( "_BlitMap",targetTex);
            } else {
                material.SetTexture( "_BlitMap",null);
            }
            if (sprite != null) {
                SetSpriteAtlasUV(sprite, material, "Main");
            }
        }

        private static void SetSpriteAtlasUV(Sprite sprite, Material material, string name)
        {
            Vector4 UVRect = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            Rect originRect = sprite.rect;
            Rect textureRect = sprite.textureRect;
            float scaleX = textureRect.width / originRect.width;
            float scaleY = textureRect.height / originRect.height;
            material.SetVector($"_{name}UVRect", UVRect);
            material.SetVector($"_{name}UVScale", new Vector4(scaleX, scaleY, 0, 0));
        }
        #endif
    }

}