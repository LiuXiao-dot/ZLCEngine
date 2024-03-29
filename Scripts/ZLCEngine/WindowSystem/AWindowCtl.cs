using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.Interfaces;
namespace ZLCEngine.WindowSystem
{
    public abstract class AWindowCtl : IWindowCtl
    {
        
        private int _id;
        private int _instanceID;
        private IWindowModel _model;
        private IWindowView _view;

        public void SetInstanceID(int instanceID)
        {
            this._instanceID = instanceID;
        }
        public void SetModel(IWindowModel model)
        {
            _model = model;
        }
        public IWindowModel GetModel()
        {
            return _model;
        }
        public void SetView(IWindowView view)
        {
            _view = view;
            _id = view.GetID();
        }
        public IWindowView GetView()
        {
            return _view;
        }
        public void Open()
        {
            SendEvent(WindowMessage.BeforeWindowEnter);
            DoOpen();
            SendEvent(WindowMessage.AfterWindowEnter);
        }

        public void Pause()
        {
            SendEvent(WindowMessage.BeforeWindowPause);
            DoPause();
            SendEvent(WindowMessage.AfterWindowPause);
        }

        public void Resume()
        {
            SendEvent(WindowMessage.BeforeWindowResume);
            DoResume();
            SendEvent(WindowMessage.AfterWindowResume);
        }

        public void Close()
        {
            SendEvent(WindowMessage.BeforeWindowExit);
            DoClose();
            _view.Close();
            SendEvent(WindowMessage.AfterWindowExit);
            ((WindowManager)IAppLauncher.Get<IWindowManager>()).ClearWindow(this);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        protected void CloseSelf()
        {
            ((WindowManager)IAppLauncher.Get<IWindowManager>()).Close(_instanceID);
        }

        /// <summary>
        /// 同一类窗口id相同
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return _id;
        }
        
        /// <summary>
        /// 实际的id，关闭窗口需要使用instanceID
        /// </summary>
        /// <returns></returns>
        public int GetInstanceID()
        {
            return _instanceID;
        }

        protected virtual void DoOpen()
        {

        }

        protected virtual void DoPause()
        {
        }

        protected virtual void DoResume()
        {

        }

        protected virtual void DoClose()
        {

        }

        private void SendEvent(WindowMessage windowMessage)
        {
            MQManager.SendEvent(MQConfigSO.WindowMessageID, windowMessage, _id);
        }
    }
}