using DesktopPanelTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Controls
{    
    /// <summary>
    /// Logique d'interaction pour WidgetStackPanelDropPlaceHolder.xaml
    /// </summary>
    public partial class WidgetStackPanelDropPlaceHolder : UserControl
    {
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
