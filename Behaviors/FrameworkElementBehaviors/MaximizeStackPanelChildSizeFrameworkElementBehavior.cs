//#define dbg

using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{

    public class MaximizeStackPanelChildSizeFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        public StackPanel Container
        {
            get { return (StackPanel)GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }

        public static readonly DependencyProperty ContainerProperty =
            DependencyProperty.Register("Container", typeof(StackPanel), typeof(MaximizeStackPanelChildSizeFrameworkElementBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            if (Container!=null)
                Container.SizeChanged += Container_SizeChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (Container != null)
                Container.SizeChanged -= Container_SizeChanged;
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetupSize();
        }

        void SetupSize()
        {
            if (Container.ActualWidth >= Container.ActualHeight)
            {
                var tw = 0d;
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"container.aw={Container.ActualWidth}");
#endif
                foreach (var element in Container.Children)
                {
                    if (element is FrameworkElement fe && fe!=AssociatedObject)
                    {
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"fe={fe} fe.aw={fe.ActualWidth}");
#endif
                        tw += fe.ActualWidth;
                    }
                }

                var w = Container.ActualWidth - tw - 8d;
                AssociatedObject.Width = w;
                AssociatedObject.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);

#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"tw={tw} container.aw={Container.ActualWidth}");
#endif
            } else
            {
                var tw = 0d;
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"container.ah={Container.ActualHeight}");
#endif
                foreach (var element in Container.Children)
                {
                    if (element is FrameworkElement fe && fe != AssociatedObject)
                    {
#if dbg
                        DesktopPanelTool.Lib.Debug.WriteLine($"fe={fe} fe.ah={fe.ActualHeight}");
#endif
                        tw += fe.ActualHeight;
                    }
                }

                var w = Container.ActualHeight - tw - 8d;
                AssociatedObject.Height = w;
                AssociatedObject.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);

#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"tw={tw} container.ah={Container.ActualHeight}");
#endif
            }
        }
    }
}
