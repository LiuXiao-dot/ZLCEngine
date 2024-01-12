using UnityEngine;
namespace ZLCEngine.TweenSystem
{
    public class MoveTweenComponent : MonoBehaviour
    {
        public MoveTween tween;
        
        public float speed;

        private void Awake()
        {
            tween.Init();
        }
    }
}