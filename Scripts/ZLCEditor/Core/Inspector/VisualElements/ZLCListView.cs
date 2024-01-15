using System;
using System.Collections;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.VisualElements
{
    public class ZLCListView : ListView
    {
        public ZLCListView(): base()
        {
            
        }
        
        public ZLCListView(
            IList itemsSource,
            float itemHeight = -1f,
            Func<VisualElement> makeItem = null,
            Action<VisualElement, int> bindItem = null)
            : base(itemsSource, itemHeight, makeItem, bindItem)
        {
        }
        
        public override void SetViewController(CollectionViewController controller)
        {
            if (this.viewController != null) {
                return;
            }
            base.SetViewController(controller);
        }
    }
}