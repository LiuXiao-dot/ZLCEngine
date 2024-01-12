using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
namespace ZLCEngine.TweenSystem
{
    public abstract class ATween : ITween
    {
        public float duration;

        public UnityAction OnFinshed;
        
        #region 运行时数据
        /// <summary>
        /// 当前播放到的时间
        /// </summary>
        private float _time;
        /// <summary>
        /// 播放速度
        /// </summary>
        private float _speed;
        /// <summary>
        /// 正在播放的动画
        /// </summary>
        private IEnumerator _update;
        #endregion

        public TweenState TweenState { get; protected set; }

        protected virtual void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            TweenState = TweenState.Idle;
        }

        [BoxGroup("工具")]
        [Button]
        public void Play(float speed = 1, float startTime = 0, bool forcePlay = false)
        {
            #if UNITY_EDITOR
            if (speed == 0) {
                Debug.LogError("动画速度不能设置为0");
                return;
            }
            #endif
            switch (TweenState) {
                case TweenState.UnInit:
                    Debug.LogError("动画尚未初始化，\n原因1:多线程调用\n原因2:使用TweenComponent但是没有调用初始化的Init方法");
                    break;
                case TweenState.Idle:
                    // 播放
                    this._speed = speed;
                    ForceUpdate(startTime); // 重置
                    TweenState = TweenState.Playing;
                    _update = Update();
                    CoroutineHelper.AddCoroutine(_update);
                    break;
                case TweenState.Playing:
                    if (forcePlay) {
                        this._speed = speed;
                        ForceUpdate(); // 重置
                    }
                    break;
                case TweenState.Pausing:
                    Debug.LogError("动画被暂停，想要重新播放请使用Resume或Reset后再调用Play");
                    break;
                case TweenState.Error:
                    Debug.LogError("动画曾有错误，想要重新播放请使用Reset后再调用Play");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        [BoxGroup("工具")]
        [Button]
        public void Pause()
        {
            if (TweenState != TweenState.Playing) return;
            if (_update != null) {
                CoroutineHelper.RemoveCoroutine(_update);
                TweenState = TweenState.Pausing;
            }
        }
        [BoxGroup("工具")]
        [Button]
        public void Resume()
        {
            if (TweenState != TweenState.Pausing) return;
            ForceUpdate(_time); // 重置
            TweenState = TweenState.Playing;
            _update = Update();
            CoroutineHelper.AddCoroutine(_update);
        }
        [BoxGroup("工具")]
        [Button]
        public void Finish()
        {
            ForceUpdate(duration);
            OnFinish();
        }

        private void OnFinish()
        {
            TweenState = TweenState.Idle;
            if (_update != null)
                CoroutineHelper.RemoveCoroutine(_update);
            OnFinshed?.Invoke();
        }

        private IEnumerator Update()
        {
            while (true) {
                var currentTime = _time + Time.deltaTime * _speed;
                if (currentTime >= duration) {
                    currentTime = duration;
                    ForceUpdate(currentTime);
                    OnFinish();
                    yield break;
                }
                ForceUpdate(currentTime);
                yield return null;
            }
        }

        [BoxGroup("工具")]
        [Button]
        public void ForceUpdate(float time = 0)
        {
            try {
                _time = time;
                OnUpdate(time);
            }
            catch (Exception e) {
                TweenState = TweenState.Error;
                Reset();
                Debug.LogError(e);
            }
        }

        public abstract void OnUpdate(float time = 0);

        [BoxGroup("工具")]
        [Button]
        public void Reset()
        {
            switch (TweenState) {
                case TweenState.UnInit:
                    Debug.LogError("动画未初始化，无法重置");
                    break;
                case TweenState.Idle:
                case TweenState.Playing:
                case TweenState.Pausing:
                case TweenState.Error:
                    TweenState = TweenState.Idle;
                    ForceUpdate();
                    if (_update != null)
                        CoroutineHelper.RemoveCoroutine(_update);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}