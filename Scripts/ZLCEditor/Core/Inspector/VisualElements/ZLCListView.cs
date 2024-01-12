using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.VisualElements
{
    public class ZLCListView : ListView
    {
        public override void SetViewController(CollectionViewController controller)
        {
            if (this.viewController != null) {
                return;
            }
            base.SetViewController(controller);
        }
    }
}