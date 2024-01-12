using System;
using System.Collections.Generic;
using UnityEngine;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
namespace ZLCEngine.TweenSystem
{
    /// <summary>
    /// 继承MonoBehaviour的tween
    /// </summary>
    public class TweenComponent : MonoBehaviour
    {
        public float speed;
        [ShowInInspector]
        public ATween tween;

        private void Awake()
        {
            tween.Init();
        }

        public IEnumerable<Type> CheckType()
        {
            var types = new List<Type>();
            AssemblyHelper.GetAllChildType<ATween>(typeof(ATween).Assembly,types);
            return types;
        }
    }
}