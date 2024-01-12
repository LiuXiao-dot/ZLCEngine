using System;
using UnityEngine;
using ZLCEngine.Utils;
namespace ZLCEngine.TweenSystem.Model
{
    /// <summary>
    /// FloatTween:
    /// 描述:
    /// </summary>
    public class FloatTween
    {
        /// <summary>
        /// 渐变start->end
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="duration"></param>
        public static Action DoFloatTween(float start,float end,float duration,Action<float> callback)
        {
            var time = 0;
            var interval = 0.02f;
            var loopTime = Mathf.CeilToInt(duration / interval);

            void DoLocalTween()
            {
                time++;
                var currentProgress = (end - start) * time / loopTime + start;
                if (time * interval >= duration) currentProgress = end;
                callback(currentProgress);
            }
            CoroutineHelper.AddCoroutineWaitTime(DoLocalTween,loopTime,interval,0);
            return DoLocalTween;
        }
    }
}