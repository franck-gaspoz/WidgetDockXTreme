//#define dbg

using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace DesktopPanelTool.Behaviors.MenuIem
{
    public class TemplatedMenuSubMenuItemBehavior
        : Behavior<MenuItem>
    {
        public Window ParentWindow
        {
            get { return (Window)GetValue(ParentWindowProperty); }
            set { SetValue(ParentWindowProperty, value); }
        }

        public static readonly DependencyProperty ParentWindowProperty =
            DependencyProperty.Register("ParentWindow", typeof(Window), typeof(TemplatedMenuSubMenuItemBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += AssociatedObject_PreviewMouseDown;
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;            
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= AssociatedObject_PreviewMouseDown;
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Initialize();
        }

        void Initialize()
        {
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"");
#endif
            var tgs = Interaction.GetTriggers(AssociatedObject);
            foreach (var trigger in tgs)
                foreach (var action in trigger.Actions.OfType<InvokeCommandAction>())
                    AssociatedObject.IsEnabled =
                        action.Command.CanExecute(action.CommandParameter);
        }

        private void AssociatedObject_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var tgs = Interaction.GetTriggers(AssociatedObject);
            foreach (var trigger in tgs)
                foreach (var action in trigger.Actions.OfType<InvokeCommandAction>())
                    action.Command.Execute(action.CommandParameter);
            ParentWindow.Close();
        }
    }
}
