//#define dbg

using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{

    public class DraggableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        Point _start;
        DataObject _dataObject;
        int _draggedElementId;
        List<string> _draggableElementTypeNames;

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(true));

        public string DraggableElementTypeNames
        {
            get { return (string)GetValue(DraggableElementTypeNamesProperty); }
            set { SetValue(DraggableElementTypeNamesProperty, value); }
        }

        public static readonly DependencyProperty DraggableElementTypeNamesProperty =
            DependencyProperty.Register("DraggableElementTypeNames", 
                typeof(string), 
                typeof(DraggableFrameworkElementBehavior), 
                new PropertyMetadata("Border,Grid,StackPanel,TextBlock"));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            if (DraggableElementTypeNames != null)
                _draggableElementTypeNames = DraggableElementTypeNames.Split(',').ToList();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching(); 
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsEnabled && CheckIsValidPosition())
            {
                Point mpos = e.GetPosition(null);
                Vector diff = _start - mpos;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) > AppSettings.MinimumHorizontalDragDistance &&
                    Math.Abs(diff.Y) > AppSettings.MinimumVerticalDragDistance)
                {
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"do drag drop");
#endif
                    //_draggedElementId = ObjectRegistry.RegisterObject(_dataObject);
                    //_dataObject = new DataObject(AssociatedObject.GetType(),_draggedElementId);
                    _dataObject = new DataObject(AssociatedObject.GetType(), AssociatedObject);
                    DragDrop.DoDragDrop(AssociatedObject, _dataObject, DragDropEffects.Move);
                }
            }
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled && CheckIsValidPosition())
            {
                _start = e.GetPosition(null);
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"init start drag position");
#endif
            }
        }

        private bool CheckIsValidPosition()
        {
            var ctrl = Mouse.DirectlyOver;

            if (ctrl is FrameworkElement e)
            {
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"ctrl={ctrl?.GetType().Name} isEnabled={IsEnabled}");
#endif
                return _draggableElementTypeNames == null || _draggableElementTypeNames.Contains(ctrl?.GetType().Name);
            }
            return false;
        }

        public static void SetIsEnabled(bool isEnabled, FrameworkElement frameworkElement)
        {
            var o = Interaction.GetBehaviors(frameworkElement)
                .OfType<DraggableFrameworkElementBehavior>()
                .FirstOrDefault();
            if (o != null)
            {
                o.IsEnabled = isEnabled;
            }
        }

        public static void SetIsEnabledDraggableDescendants(DependencyObject root,Type descendantType,bool isEnabled)
        {
#if dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"set isEnable {descendantType.Name} isEnabled={isEnabled}");
#endif
            if (descendantType != null)
            {
                var draggableElements = WPFUtil.FindChilds(descendantType, root);
                foreach (var de in draggableElements)
                    DraggableFrameworkElementBehavior.SetIsEnabled(isEnabled, (FrameworkElement)de);
            }
        }

    }
}
