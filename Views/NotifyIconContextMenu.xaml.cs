using DesktopPanelTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Views
{
    /// <summary>
    /// Logique d'interaction pour NotifyIconContextMenu.xaml
    /// </summary>
    public partial class NotifyIconContextMenu : Window
    {
        public NotifyIconContextMenu(DesktopPanelToolViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}
