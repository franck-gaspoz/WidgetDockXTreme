//#define dbg

using DesktopPanelTool.Models;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class DroppableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        public string AcceptableDropDataTypesList
        {
            get { return (string)GetValue(AcceptableDropDataTypesListProperty); }
            set { SetValue(AcceptableDropDataTypesListProperty, value); }
        }

        public static readonly DependencyProperty AcceptableDropDataTypesListProperty =
            DependencyProperty.Register("AcceptableDropDataTypesList", typeof(string), typeof(DroppableFrameworkElementBehavior), new PropertyMetadata(null));

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
            AssociatedObject.DragEnter += AssociatedObject_DragEnter;
            AssociatedObject.DragOver += AssociatedObject_DragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AllowDrop = false;
            AssociatedObject.Drop -= AssociatedObject_Drop;
            AssociatedObject.DragEnter -= AssociatedObject_DragEnter;
            AssociatedObject.DragOver -= AssociatedObject_DragOver;
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            CheckIsAcceptableDrop(sender,e);
        }

        private void AssociatedObject_DragEnter(object sender, DragEventArgs e)
        {
            CheckIsAcceptableDrop(sender, e);
        }

        void CheckIsAcceptableDrop(object sender,DragEventArgs e)
        {
            if (AcceptableDropDataTypesList!=null)
            {
                var types = AcceptableDropDataTypesList.Split(",");
                foreach (var typename in types)
                {
                    if (typename == "*")
                        return;
                    var type = GetType(typename);
                    if (type!=null && e.Data.GetDataPresent(type))
                        return;
                }
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        Type GetType(string typeName)
        {
            return Type.GetType(typeName);
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
#if alldbg || dbg
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
