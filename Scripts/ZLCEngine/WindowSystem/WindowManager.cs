using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLCEngine.ApplicationSystem;
using ZLCEngine.EventSystem;
using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.Interfaces;
using ZLCEngine.Utils;
using Event = ZLCEngine.EventSystem.Event;
namespace ZLCEngine.WindowSystem
{
    /// <summary>
    ///     窗口管理器
    /// 每个窗口都有独特的id，与窗口id，独特的id用于区分实例，窗口id用来区分窗口类别
    /// 实例id=窗口id+从1开始往上加，选取一个当未使用的数字
    /// </summary>
    public class WindowManager : IWindowManager, ISubscriber
    {
        /// <summary>
        ///     窗口栈
        /// 按窗口层级分不同的栈
        /// </summary>
        //private Stack<IWindowCtl>[] _coreCtlStack = new Stack<IWindowCtl>[3];
        /// <summary>
        /// 所有Core窗口的顺序列表
        /// </summary>
        private List<IWindowCtl> _coreCtlList = new List<IWindowCtl>();

        private List<IWindowCtl> _extraCtlList = new List<IWindowCtl>();

        private List<IWindowCtl> _ignoreCtlList = new List<IWindowCtl>();
        private WindowLayer _lastLayer;
        /// <summary>
        ///     各个层级的根Transform
        /// </summary>
        private RectTransform[] _layerRoots;

        private IResLoader _resLoader;
        /// <summary>
        ///     总的根Transform
        /// </summary>
        private RectTransform _root;

        private IWindowConfig _windowConfig;

        /// <summary>
        /// </summary>
        /// <param name="subEvent"></param>
        public void OnMessage(Event subEvent)
        {
            switch (((AMQ)subEvent.sender).id) {
                case MQConstant.RES_MQ:
                    switch ((SceneMessage)subEvent.operate) {
                        case SceneMessage.OnSceneOpen:
                            // 场景打开
                            // 判断是否加载了UI场景，并在UI场景中初始化WindowManager
                            if (subEvent.data.Equals(AppConfigSO.Instance.uiSceneName)) {
                                MQManager.Unsubscribe(MQConfigSO.SceneMessageID, this, SceneMessage.OnSceneOpen);
                                Scene uiScene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(AppConfigSO.Instance.uiSceneName));
                                GameObject[] uiScenePrefab = uiScene.GetRootGameObjects();
                                _root = (RectTransform)uiScenePrefab[0].transform.Find("WindowManager");
                                // 初始化层级
                                var layerNames = Enum.GetNames(typeof(WindowLayer));
                                var length = layerNames.Length;
                                _layerRoots = new RectTransform[length];
                                for (int i = 0; i < length; i++) {
                                    var layerRootGo = new GameObject(layerNames[i]);
                                    var rectTransform = layerRootGo.AddComponent<RectTransform>();
                                    _layerRoots[i] = rectTransform;
                                    rectTransform.SetParent(_root);
                                    rectTransform.localScale = Vector3.one;
                                    rectTransform.localPosition = Vector3.zero;
                                    RectTransformHelper.SetStretchStretch(rectTransform);
                                }
                            }
                            break;
                    }
                    break;
            }
            subEvent.Callback();
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="onProgress"></param>
        public void Load(Action<int, int> onProgress = null)
        {
            _windowConfig = IWindowConfig.Instance;
            if (_windowConfig == null) {
                Debug.LogError("未初始化IWindowConfig");
                return;
            }
            _resLoader = IResLoader.Instance;
            onProgress?.Invoke(1, 1);
        }

        /// <inheritdoc />
        public void Init()
        {
            // 参看MQConfigSO.internalMQS
            MQManager.Subscribe(MQConfigSO.SceneMessageID, this, SceneMessage.OnSceneOpen);
        }


        /// <summary>
        ///     打开窗口
        /// </summary>
        public int Open(int id, IWindowModel windowModel = null)
        {
            _resLoader.LoadAssetSync(_windowConfig.GetWindowPath(id), out GameObject go);
            if (go == null) {
                Debug.LogError($"窗口{id}没有对应的prefab");
                return -1;
            }
            MQManager.SendEvent(MQConfigSO.WindowMessageID, WindowMessage.AfterWindowLoaded, id);

            AWindowView view = go.GetComponent<AWindowView>();
            WindowLayer layer = view.windowLayer;
            IWindowCtl ctl = _windowConfig.CreateWindowCtl(id);
            switch (layer) {
                case WindowLayer.MAIN:
                case WindowLayer.CHILD:
                case WindowLayer.SMALL:
                    OnOpenCoreWindow(layer);
                    _coreCtlList.Add(ctl);
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                    OnOpenExtraWindow();
                    _extraCtlList.Add(ctl);
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                    OnOpenIgnoreWindow();
                    _ignoreCtlList.Add(ctl);
                    break;
                default:
                    Debug.LogError($"错误的窗口层级类型，窗口ID:{id}");
                    break;
            }
            var instanceID = CreateInstanceID(id, layer);
            view = OpenView(view); // 此处返回的是实例化后的view
            ctl.SetInstanceID(instanceID);
            ctl.SetModel(windowModel);
            ctl.SetView(view);
            ctl.Open();
            return instanceID;
        }

        /// <summary>
        ///     关闭窗口
        /// </summary>
        public void Close(int instanceID)
        {
            var id = GetIDFromInstanceID(instanceID);
            var layer = GetWindowLayerFromID(id);

            // 如果该界面是最上层界面直接退出
            // 否则，如果是被弹窗、面板覆盖，则先关掉这些弹窗，再关掉该界面
            // 如果中间有其他界面，则关闭失败
            // 如果没有该界面，关闭失败
            switch (layer) {
                case WindowLayer.MAIN:
                case WindowLayer.CHILD:
                case WindowLayer.SMALL:
                    CloseCoreWindow(instanceID, layer);
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                {
                    var index = _extraCtlList.FindIndex(t => t.GetInstanceID() == instanceID);
                    if (index == -1)
                        return;
                    var windowCtl = _extraCtlList[index];
                    CloseIgnoreWindow(instanceID);
                    windowCtl.Close();
                }
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                {
                    var index = _ignoreCtlList.FindIndex(t => t.GetInstanceID() == instanceID);
                    if (index == -1)
                        return;
                    var windowCtl = _ignoreCtlList[index];
                    CloseIgnoreWindow(instanceID);
                    windowCtl.Close();
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        ~WindowManager()
        {
            Dispose();
        }

        /// <summary>
        ///     打开窗口，在此之前只是加载了窗口资源，并未实例化
        /// </summary>
        private AWindowView OpenView(AWindowView view)
        {
            int layerIndex = (int)view.windowLayer;
            GameObject instance = GameObject.Instantiate(view.gameObject, _layerRoots[layerIndex]);
            view = instance.GetComponent<AWindowView>();
            RefreshSortingOrder(view);
            return view;
        }

        private int CreateInstanceID(int windowID, WindowLayer layer)
        {
            var index = 1;
            switch (layer) {
                case WindowLayer.MAIN:
                case WindowLayer.CHILD:
                case WindowLayer.SMALL:
                    foreach (var windowCtl in _coreCtlList) {
                        if (windowCtl.GetID() == windowID)
                            index++;
                    }
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                    foreach (var windowCtl in _extraCtlList) {
                        if (windowCtl.GetID() == windowID)
                            index++;
                    }
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                    foreach (var windowCtl in _ignoreCtlList) {
                        if (windowCtl.GetID() == windowID)
                            index++;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
            return windowID + index;
        }

        /// <summary>
        ///     刷新受影响窗口的sorting order
        /// </summary>
        private void RefreshSortingOrder(AWindowView view)
        {
            Canvas canvas = view.GetComponent<Canvas>();
            switch (view.windowLayer) {
                case WindowLayer.MAIN:
                case WindowLayer.CHILD:
                case WindowLayer.SMALL:
                    // 底层的窗口会被隐藏，所以按照默认层级就可以了
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = Constant.sortingOrders[view.windowLayer];
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                    // 需要与核心窗口同层级
                    canvas.overrideSorting = true;
                    AWindowView curView = _coreCtlList.Last().GetView() as AWindowView;
                    canvas.sortingOrder = curView.GetComponent<Canvas>().sortingOrder;
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                    // 默认层级
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = Constant.sortingOrders[view.windowLayer];
                    break;
                default:
                    Debug.LogError($"错误的WindowLayer{view.windowLayer} 窗口:{view.gameObject.name}");
                    break;
            }
        }

        /// <summary>
        ///     打开主窗口
        ///     1.隐藏其他主窗口、子窗口、小窗口并关闭对应窗口上的弹窗和面板
        /// </summary>
        /// <param name="layer"></param>
        private void OnOpenCoreWindow(WindowLayer layer)
        {
            int length = _extraCtlList.Count;
            for (int i = length - 1; i >= 0; i--) {
                // 倒序依次关闭
                CloseExtraWindow(_extraCtlList[i].GetInstanceID());
            }

            IListHelper.ReverseForeach(_coreCtlList, ctl =>
            {
                var ctlLayer = ((AWindowView)ctl.GetView()).windowLayer;
                if (ctlLayer < layer) {
                    return false;
                }
                switch (ctlLayer) {

                    case WindowLayer.MAIN:
                    case WindowLayer.CHILD:
                        ctl.Pause();
                        (ctl.GetView() as AWindowView).gameObject.SetActive(false);
                        break;
                    case WindowLayer.SMALL:
                        ctl.Close();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
                }
                if (ctlLayer == layer) {
                    return false;
                }
                return true;
            });
        }

        private void OnOpenExtraWindow()
        {
            // 暂不影响任何窗口
        }

        private void OnOpenIgnoreWindow()
        {
            // 暂不影响任何窗口
        }

        /// <summary>
        /// 关闭相关窗口，并恢复底层窗口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void CloseCoreWindow(int id, WindowLayer layer)
        {
            if (_coreCtlList.All(t => t.GetInstanceID() != id)) {
                // 如果没有目标窗口，则什么也不做
                return;
            }
            // 关闭全部extra窗口
            IListHelper.ReverseForeach(_extraCtlList, ctl => ctl.Close());
            
            // 一直关闭到目标窗口
            IListHelper.ReverseForeach(_coreCtlList, ctl =>
            {
                ctl.Close();
                // 碰到要关闭的窗口后退出不再关闭其他窗口
                return id != ctl.GetInstanceID();
            });
            // 恢复上层界面
            IListHelper.ReverseForeach(_coreCtlList, ctl =>
            {
                var tempLayer = (WindowLayer)ctl.GetView().GetWindowLayer();
                // 大于等于自己layer的需要调用一次恢复，碰到小于等于自己layer的就停止
                if (tempLayer >= layer) {
                    (ctl.GetView() as AWindowView).gameObject.SetActive(true);
                    ctl.Resume();
                }

                return tempLayer > layer;
            });

            // 忽略ignore窗口
        }

        /// <summary>
        ///     随时关闭，不影响其他窗口
        /// </summary>
        private void CloseExtraWindow(int instanceID)
        {
            int index = _extraCtlList.FindIndex(t => t.GetInstanceID() == instanceID);
            if (index != -1) {
                _extraCtlList[index].Close();
            }
        }

        /// <summary>
        ///     随时关闭，不影响其他窗口
        /// </summary>
        private void CloseIgnoreWindow(int instanceID)
        {
            int index = _ignoreCtlList.FindIndex(t => t.GetInstanceID() == instanceID);
            if (index != -1) {
                _ignoreCtlList[index].Close();
            }
        }

        /// <summary>
        ///     清除关闭后的view和ctl
        /// </summary>
        internal void ClearWindow(IWindowCtl ctl)
        {
            AWindowView view = (AWindowView)ctl.GetView();
            WindowLayer windowLayer = view.windowLayer;
            switch (windowLayer) {
                case WindowLayer.MAIN:
                case WindowLayer.CHILD:
                case WindowLayer.SMALL:
                    IWindowCtl curCtl = _coreCtlList.Last();
                    if (curCtl == ctl) {
                        _coreCtlList.Remove(curCtl);
                    } else {
                        Debug.LogError("逻辑错误,要清除的窗口不在栈顶");
                    }
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                    _extraCtlList.Remove(ctl);
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                    _ignoreCtlList.Remove(ctl);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public int GetIDFromInstanceID(int instanceID)
        {
            return instanceID - instanceID % Constant.LayerRatio;
        }

        public WindowLayer GetWindowLayerFromID(int id)
        {
            return (WindowLayer)(id / Constant.LayerRatio);
        }
    }
}