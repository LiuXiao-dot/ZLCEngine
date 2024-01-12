using System;
using UnityEngine;
namespace ZLCEngine.TweenSystem
{
    /// <summary>
    /// 移动动画
    /// 1.在duration时间内，根据curve的曲线，执行从from移动到to的逻辑，注意：如果curve最终值不是1，将不会移动到to的位置
    /// </summary>
    [Serializable]
    public class MoveTween : ATween
    {
        public Vector3 from;
        public Vector3 to;
        public AnimationCurve curve;
        [SerializeField]private Transform _transform;

        public MoveTween()
        {
            TweenState = TweenState.UnInit;
        }

        public MoveTween(Transform transform)
        {
            TweenState = TweenState.UnInit;
            _transform = transform;
            base.Init();
        }

        public override void OnUpdate(float time = 0)
        {
            try {
                _transform.localPosition = Vector3.LerpUnclamped(from, to, curve.Evaluate(time));
            }
            catch (Exception e){
                TweenState = TweenState.Error;
                Reset();
                Debug.LogError(e);
            }
        }
       
    }
}