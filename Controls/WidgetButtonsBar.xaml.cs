using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Controls
{
    /// <summary>
    /// Logique d'interaction pour WidgetButtonsBar.xaml
    /// </summary>
    public partial class WidgetButtonsBar : UserControl
    {
        public bool ButtonSettingsVisible
        {
            get { return (bool)GetValue(ButtonSettingsVisibleProperty); }
            set { SetValue(ButtonSettingsVisibleProperty, value); }
        }

        public static readonly DependencyProperty ButtonSettingsVisibleProperty =
            DependencyProperty.Register("ButtonSettingsVisible", typeof(bool), typeof(WidgetButtonsBar), new PropertyMetadata(true));

        public WidgetButtonsBar()
        {
            InitializeComponent();
        }
    }
}
