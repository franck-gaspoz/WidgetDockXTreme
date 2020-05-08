//#define dbg

using DesktopPanelTool.Behaviors.FrameworkElementBehaviors;
using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    internal class MovableTransparentWindowBehavior
        : Behavior<Window>
    {
        public static Cursor CursorDefault = Cursors.Arrow;
        public static Cursor CursorDragging = Cursors.Hand;
        public static double NotMovableEdgeSize = 8;
        public bool IsMovable = true;

        public Type TypeControlUnderMouseAvoidingMove
        {
            get { return (Type)GetValue(TypeControlUnderMouseAvoidingMoveProperty); }
            set { SetValue(TypeControlUnderMouseAvoidingMoveProperty, value); }
        }

        public static readonly DependencyProperty TypeControlUnderMouseAvoidingMoveProperty =
            DependencyProperty.Register("TypeControlUnderMouseAvoidingMove", typeof(Type), typeof(MovableTransparentWindowBehavior), new PropertyMetadata(null));

        /// <summary>
        /// no more than 1/MaximumMoveEventRate*1000 window position changed per second
        /// </summary>
        public static double MaximumMoveEventRate = 20;

        double InitialPX, InitialPY;
        DateTime LastDragMoveTime;

        public bool IsDragging { get; protected set; } = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
            AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
        }

        bool AcceptMousePosition(Point p)
        {
            var m = AssociatedObject.Margin;
            return IsMovable && 
                ((
                p.X > NotMovableEdgeSize+m.Left &&
                p.X < AssociatedObject.Width - NotMovableEdgeSize - m.Right &&
                p.Y > NotMovableEdgeSize + m.Top &&
                p.Y < AssociatedObject.Height - NotMovableEdgeSize - m.Bottom
                ) &&
                ( TypeControlUnderMouseAvoidingMove == null ||
                  (Mouse.DirectlyOver is DependencyObject o &&
                  !WPFHelper.HasParent(TypeControlUnderMouseAvoidingMove,o))
                )
                );
        }

        bool InInterval(double v, double min, double length) => (v >= min) && (v <= (min + length));

        private void AssociatedObject_MouseMove(
            object sender, 
            MouseEventArgs e)
        {
            if (IsDragging)
            {
                var elapsed = DateTime.Now - LastDragMoveTime;
                if (elapsed.TotalMilliseconds < MaximumMoveEventRate)
                    return;
                var p = e.GetPosition(AssociatedObject);
#if false && dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"ix={InitialPX},iy={InitialPY} x={p.X},y={p.Y}");
#endif
                var dx = p.X - InitialPX;
                var dy = p.Y - InitialPY;
                AssociatedObject.Left += dx;
                AssociatedObject.Top += dy;
                LastDragMoveTime = DateTime.Now;
            }
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(AssociatedObject);
            if (e.ChangedButton == MouseButton.Left
                && AcceptMousePosition(p))
            {
                SetUpIsMoving(new Point(p.X,p.Y));                
            }
        }

        void SetUpIsMoving(Point setUpIsMovingFrom)
        {            
            LastDragMoveTime = DateTime.Now;
            AssociatedObject.Cursor = CursorDragging;
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"start drag move: x={setUpIsMovingFrom.X},y={setUpIsMovingFrom.Y}");
#endif            
            InitialPX = setUpIsMovingFrom.X;
            InitialPY = setUpIsMovingFrom.Y;
            Mouse.Capture(AssociatedObject);
            ResizableTransparentWindowBehavior.DisableResizableBehavior(AssociatedObject);
            DraggableFrameworkElementBehavior.SetIsEnabledDraggableDescendants(
                AssociatedObject,TypeControlUnderMouseAvoidingMove,false);
            IsDragging = true;
        }

        /*void SetIsEnabledDraggableDescendants(bool isEnabled)
        {
            if (TypeControlUnderMouseAvoidingMove != null)
            {
                var draggableElements = WPFUtil.FindChilds(TypeControlUnderMouseAvoidingMove, AssociatedObject);
                foreach (var de in draggableElements)
                    DraggableFrameworkElementBehavior.SetIsEnabled(isEnabled, (FrameworkElement)de);
            }
        }*/

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && IsDragging)
            {

#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"end drag move");
#endif
                IsDragging = false;
                Mouse.Capture(null);
                AssociatedObject.Cursor = CursorDefault;
                ResizableTransparentWindowBehavior.EnableResizableBehavior(AssociatedObject);
                DraggableFrameworkElementBehavior.SetIsEnabledDraggableDescendants(
                    AssociatedObject, TypeControlUnderMouseAvoidingMove, true);
            }
        }

        public static void DisableMovableBehavior(Window win)
        {
            var o = Interaction.GetBehaviors(win)
                .OfType<MovableTransparentWindowBehavior>()
                .FirstOrDefault();
            if (o != null)
                o.IsMovable = false;
        }

        public static void EnableMovableBehavior(
            Window win,
            Point? setUpIsMovingFrom = null)
        {
            var o = Interaction.GetBehaviors(win)
                .OfType<MovableTransparentWindowBehavior>()
                .FirstOrDefault();
            if (o != null)
            {
                o.IsMovable = true;
                if (setUpIsMovingFrom.HasValue)
                    o.SetUpIsMoving(setUpIsMovingFrom.Value);
            }
        }

    }
}