#define dbg

using DesktopPanelTool.Models;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class DroppableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        public ICommand DropHandlerCommand
        {
            get { return (ICommand)GetValue(DropHandlerCommandProperty); }
            set { SetValue(DropHandlerCommandProperty, value); }
        }

        public static readonly DependencyProperty DropHandlerCommandProperty =
            DependencyProperty.Register("DropHandlerCommand", typeof(ICommand), typeof(DroppableFrameworkElementBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += AssociatedObject_Drop;            
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AllowDrop = false;
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"dropping");
#endif
            if (DropHandlerCommand != null)
            {
                var dragComponentData = new DragData(e.Data, e, AssociatedObject);
                if (DropHandlerCommand.CanExecute(dragComponentData))
                    DropHandlerCommand.Execute(dragComponentData);
            }
        }
    }
}
