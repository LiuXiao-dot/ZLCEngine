using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLCEngine.ApplicationSystem;
using ZLCEngine.EventSystem;
using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.Interfaces;
using Event = ZLCEngine.EventSystem.Event;
namespace ZLCEngine.WindowSystem
{
    /// <summary>
    ///     窗口管理器
    /// </summary>
    /// ~
    public class WindowManager : IWindowManager, ISubscriber<int>
    {

        /// <summary>
        ///     窗口栈
        /// </summary>
        private Stack<IWindowCtl> _coreCtlStack;
        private Stack<AWindowView> _coreViewStack;

        private List<IWindowCtl> _extraCtlList;
        private List<IWindowView> _extraViewList;

        private List<IWindowCtl> _ignoreCtlList;
        private List<IWindowView> _ignoreViewList;
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
                case 1:
                    switch (subEvent.operate) {
                        case 0:
                            // 场景打开
                            // 判断是否加载了UI场景，并在UI场景中初始化WindowManager
                            if (subEvent.data.Equals(AppConfigSO.Instance.uiSceneName)) {
                                MQManager.Unsubscribe(MQConfigSO.SceneMessageID, this, SceneMessage.OnSceneOpen);
                                Scene uiScene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(AppConfigSO.Instance.uiSceneName));
                                GameObject[] uiScenePrefab = uiScene.GetRootGameObjects();
                                _root = (RectTransform)uiScenePrefab[0].transform.Find("WindowManager");
                            }
                            break;
                    }
                    break;
            }
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
        public void Open(int id, IWindowModel windowModel = null)
        {
            _resLoader.LoadAssetSync(GetWindowPath(id), out GameObject go);
            if (go == null) {
                Debug.LogError($"窗口{id}没有对应的prefab");
                return;
            }
            MQManager.SendEvent(MQConfigSO.WindowMessageID, WindowMessage.AfterWindowLoaded, id);

            AWindowView view = go.GetComponent<AWindowView>();
            WindowLayer layer = view.windowLayer;
            switch (layer) {
                case WindowLayer.MAIN:
                case WindowLayer.CHILD:
                case WindowLayer.SMALL:
                    OnOpenCoreWindow(layer);
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                    OnOpenExtraWindow();
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                    OnOpenIgnoreWindow();
                    break;
                default:
                    Debug.LogError($"错误的窗口层级类型，窗口ID:{id}");
                    break;
            }
            IWindowCtl ctl = _windowConfig.CreateWindowCtl(id);
            _coreCtlStack.Push(ctl);
            view = OpenView(view); // 此处返回的是实例化后的view
            ctl.SetModel(windowModel);
            ctl.SetView(view);
            ctl.Open();
        }


        /// <summary>
        ///     关闭窗口
        /// </summary>
        public void Close(int id)
        {
            if (!_resLoader.CheckAsset(GetWindowPath(id), out GameObject go)) {
                return;
            }

            AWindowView prefab = go.GetComponent<AWindowView>();
            WindowLayer layer = prefab.windowLayer;
            bool canClose;
            // 如果该界面是最上层界面直接退出
            // 否则，如果是被弹窗、面板覆盖，则先关掉这些弹窗，再关掉该界面
            // 如果中间有其他界面，则关闭失败
            // 如果没有该界面，关闭失败
            switch (layer) {
                case WindowLayer.MAIN:
                case WindowLayer.CHILD:
                case WindowLayer.SMALL:
                    canClose = CloseCoreWindow(id, layer);
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                    canClose = CloseExtraWindow(id);
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                    canClose = CloseIgnoreWindow(id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (!canClose) return;

        }
        ~WindowManager()
        {
            Dispose();
        }
#region UTILS
        public string GetWindowPath(int id)
        {
            return $"{_windowConfig.GetWindowPath(id)}.prefab";
        }
#endregion

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
                    AWindowView curView = _coreViewStack.Peek();
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
                CloseExtraWindow(_extraCtlList[i].GetID());
            }

            foreach (IWindowCtl ctl in _coreCtlStack) {
                if (((AWindowView)ctl.GetView()).windowLayer > layer) {
                    return;
                }
                ctl.Pause();
                return;
            }
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
        ///     返回true表示可以正常退出，否则无法退出
        /// </summary>
        /// <param name="id"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private bool CloseCoreWindow(int id, WindowLayer layer)
        {
            foreach (IWindowCtl ctl in _coreCtlStack) {
                // 遍历栈
                AWindowView view = (AWindowView)ctl.GetView();
                switch (view.windowLayer) {
                    case WindowLayer.MAIN:
                    case WindowLayer.CHILD:
                    case WindowLayer.SMALL:
                        if (view.windowLayer <= layer)
                            return view.GetID() == id;
                        return false;
                    case WindowLayer.POPUP:
                    case WindowLayer.PANEL:
                        // 全部关闭
                        ctl.Close();
                        break;
                    case WindowLayer.LOADING:
                    case WindowLayer.MASK:
                    case WindowLayer.TIP:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }

        /// <summary>
        ///     随时关闭，不影响其他窗口
        /// </summary>
        private bool CloseExtraWindow(int id)
        {
            int index = _extraViewList.FindIndex(t => t.GetID() == id);
            return index != -1;
        }

        /// <summary>
        ///     随时关闭，不影响其他窗口
        /// </summary>
        private bool CloseIgnoreWindow(int id)
        {
            int index = _ignoreViewList.FindIndex(t => t.GetID() == id);
            return index != -1;
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
                    IWindowCtl curCtl = _coreCtlStack.Peek();
                    if (curCtl == ctl) {
                        _coreCtlStack.Pop();
                        _coreViewStack.Pop();
                    }
                    break;
                case WindowLayer.POPUP:
                case WindowLayer.PANEL:
                    _extraCtlList.Remove(ctl);
                    _extraViewList.Remove(view);
                    break;
                case WindowLayer.LOADING:
                case WindowLayer.MASK:
                case WindowLayer.TIP:
                    _ignoreCtlList.Remove(ctl);
                    _ignoreViewList.Remove(view);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}