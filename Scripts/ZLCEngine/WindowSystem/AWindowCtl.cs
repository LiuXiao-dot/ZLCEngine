using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.Interfaces;
namespace ZLCEngine.WindowSystem
{
    public abstract class AWindowCtl : IWindowCtl
    {
        private IWindowModel _model;
        private IWindowView _view;
        private int _id;

        public void SetModel(IWindowModel model)
        {
            _model = model;
        }
        public IWindowModel GetWindowModel()
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

        protected virtual void DoOpen()
        {
            
        }
        
        public void Pause()
        {
            SendEvent(WindowMessage.BeforeWindowPause);
            DoPause();
            SendEvent(WindowMessage.AfterWindowPause);
        }
        
        protected virtual void DoPause()
        {
        }

        public void Resume()
        {
            SendEvent(WindowMessage.BeforeWindowResume);
            DoResume();
            SendEvent(WindowMessage.AfterWindowResume);
        }

        protected virtual void DoResume()
        {
            
        }
        
        public void Close()
        {
            SendEvent(WindowMessage.BeforeWindowExit);
            DoClose();
            _view.Close();
            SendEvent(WindowMessage.AfterWindowExit);
            ((WindowManager)IAppLauncher.Get<IWindowManager>()).ClearWindow(this);
        }

        protected virtual void DoClose()
        {
            
        }
        
        public int GetID()
        {
            return _id;
        }

        private void SendEvent(WindowMessage windowMessage)
        {
            MQManager.SendEvent(MQConfigSO.WindowMessageID, windowMessage, _id);
        }
    }
}