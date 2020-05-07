using System.Windows;

namespace DesktopPanelTool.Models
{
    internal class DragData
    {
        public IDataObject DataObject { get; protected set; }
        public DragEventArgs DragEventArgs { get; protected set; }
        public FrameworkElement Target { get; protected set; }

        public DragData(IDataObject dataObject, DragEventArgs dragEventArgs, FrameworkElement target)
        {
            DataObject = dataObject;
            DragEventArgs = dragEventArgs;
            this.Target = target;
        }
    }
}
