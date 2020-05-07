using DesktopPanelTool.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopPanelTool.Controls
{    
    /// <summary>
    /// Logique d'interaction pour WidgetStackPanelDropPlaceHolder.xaml
    /// </summary>
    public partial class WidgetStackPanelDropPlaceHolder : UserControl
    {
        public ResourceKey DropSensitiveAreaHighlightBackgroundKey
        {
            get { return (ResourceKey)GetValue(DropSensitiveAreaHighlightBackgroundKeyProperty); }
            set { SetValue(DropSensitiveAreaHighlightBackgroundKeyProperty, value); }
        }

        public static readonly DependencyProperty DropSensitiveAreaHighlightBackgroundKeyProperty =
            DependencyProperty.Register("DropSensitiveAreaHighlightBackgroundKey", typeof(ResourceKey), typeof(WidgetStackPanelDropPlaceHolder), new PropertyMetadata( null ));

        public Brush DropSensitiveAreahighlightBackgroundBrush
        {
            get { return (Brush)GetValue(DropSensitiveAreahighlightBackgroundBrushProperty); }
            set { SetValue(DropSensitiveAreahighlightBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty DropSensitiveAreahighlightBackgroundBrushProperty =
            DependencyProperty.Register("DropSensitiveAreahighlightBackgroundBrush", typeof(Brush), typeof(WidgetStackPanelDropPlaceHolder), new PropertyMetadata(null));

        public Thickness PlaceHolderMargin
        {
            get { return (Thickness)GetValue(PlaceHolderMarginProperty); }
            set { SetValue(PlaceHolderMarginProperty, value); }
        }

        public static readonly DependencyProperty PlaceHolderMarginProperty =
            DependencyProperty.Register("PlaceHolderMargin", typeof(Thickness), typeof(WidgetStackPanelDropPlaceHolder), new PropertyMetadata(new Thickness(0d)));

        public WidgetStackPanelDropPlaceHolderViewModel ViewModel { get; set; }

        public WidgetStackPanelDropPlaceHolder()
        {
            InitializeComponent();
        }

        public WidgetStackPanelDropPlaceHolder(WidgetStackPanelDropPlaceHolderViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }
    }
}
