using ZLCEngine.EventSystem.MessageQueue;
using ZLCEngine.Interfaces;
namespace ZLCEngine.WindowSystem
{
    public abstract class AWindowCtl : IWindowCtl
    {
        private int _id;
        private IWindowModel _model;
        private IWindowView _view;

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

        public int GetID()
        {
            return _id;
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