using DesktopPanelTool.Lib;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DesktopPanelTool.Controls
{
    /// <summary>
    /// Logique d'interaction pour IconButton.xaml
    /// </summary>
    public partial class IconButton : UserControl
    {
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(IconButton), new PropertyMetadata(null));

        public object ButtonToolTip
        {
            get { return (object)GetValue(ButtonToolTipProperty); }
            set { SetValue(ButtonToolTipProperty, value); }
        }

        public static readonly DependencyProperty ButtonToolTipProperty =
            DependencyProperty.Register("ButtonToolTip", typeof(object), typeof(IconButton), new PropertyMetadata(null));

        public double MouseOutOpacity
        {
            get { return (double)GetValue(MouseOutOpacityProperty); }
            set { SetValue(MouseOutOpacityProperty, value); }
        }

        public static readonly DependencyProperty MouseOutOpacityProperty =
            DependencyProperty.Register("MouseOutOpacity", typeof(double), typeof(IconButton), new PropertyMetadata(0.5d));

        public double MouseOverOpacity
        {
            get { return (double)GetValue(MouseOverOpacityProperty); }
            set { SetValue(MouseOverOpacityProperty, value); }
        }

        public static readonly DependencyProperty MouseOverOpacityProperty =
            DependencyProperty.Register("MouseOverOpacity", typeof(double), typeof(IconButton), new PropertyMetadata(1d));

        public double MouseOverOpacityTransitionAnimationDuration
        {
            get { return (double)GetValue(MouseOverOpacityTransitionAnimationDurationProperty); }
            set { SetValue(MouseOverOpacityTransitionAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty MouseOverOpacityTransitionAnimationDurationProperty =
            DependencyProperty.Register("MouseOverOpacityTransitionAnimationDuration", typeof(double), typeof(IconButton), new PropertyMetadata(100d));

        public Cursor MouseOverCursor
        {
            get { return (Cursor)GetValue(MouseOverCursorProperty); }
            set { SetValue(MouseOverCursorProperty, value); }
        }
        public static readonly DependencyProperty MouseOverCursorProperty =
            DependencyProperty.Register("MouseOverCursor", typeof(Cursor), typeof(IconButton), new PropertyMetadata(Cursors.Arrow));

        public object EventObjectHandler
        {
            get { return (object)GetValue(EventObjectHandlerProperty); }
            set { SetValue(EventObjectHandlerProperty, value); }
        }

        public static readonly DependencyProperty EventObjectHandlerProperty =
            DependencyProperty.Register("EventObjectHandler", typeof(object), typeof(IconButton), new PropertyMetadata(new DefaultInteractionEventHandler()));

        public string ClickHandlerMethodName
        {
            get { return (string)GetValue(ClickHandlerMethodNameProperty); }
            set { SetValue(ClickHandlerMethodNameProperty, value); }
        }

        public static readonly DependencyProperty ClickHandlerMethodNameProperty =
            DependencyProperty.Register("ClickHandlerMethodName", typeof(string), typeof(IconButton), new PropertyMetadata(nameof(DefaultInteractionEventHandler.ClickEventHandler)));

        public ICommand ClickHandlerCommand
        {
            get { return (ICommand)GetValue(ClickHandlerCommandProperty); }
            set { SetValue(ClickHandlerCommandProperty, value); }
        }

        public static readonly DependencyProperty ClickHandlerCommandProperty =
            DependencyProperty.Register("ClickHandlerCommand", typeof(ICommand), typeof(IconButton), new PropertyMetadata(null));

        public object ClickHandlerCommandParameter
        {
            get { return (object)GetValue(ClickHandlerCommandParameterProperty); }
            set { SetValue(ClickHandlerCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty ClickHandlerCommandParameterProperty =
            DependencyProperty.Register("ClickHandlerCommandParameter", typeof(object), typeof(IconButton), new PropertyMetadata(null));

        public bool EnableKeepOrientationWhenInRotationOrientedPanelBehavior
        {
            get { return (bool)GetValue(EnableKeepOrientationWhenInRotationOrientedPanelBehaviorProperty); }
            set { SetValue(EnableKeepOrientationWhenInRotationOrientedPanelBehaviorProperty, value); }
        }

        public static readonly DependencyProperty EnableKeepOrientationWhenInRotationOrientedPanelBehaviorProperty =
            DependencyProperty.Register("EnableKeepOrientationWhenInRotationOrientedPanelBehavior", typeof(bool), typeof(IconButton), new PropertyMetadata(false));

        public IconButton()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
