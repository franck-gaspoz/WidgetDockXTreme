using DesktopPanelTool.Controls;
using DesktopPanelTool.Lib;
using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace DesktopPanelTool.ViewModels
{
    [Serializable]
    public class WidgetBaseViewModel
        : ViewModelBase, ISerializable
    {
        public WidgetControl View { get; protected set; }
        public DesktopPanelBaseViewModel PanelViewModel { get; set; }
        public AutoSizableElementViewModel AutoSizableElementViewModel { get; set; }

        #region border geometry

        public GridLength Widget_BorderSize_GridLength
            => new GridLength((double)Application.Current.Resources["Widget_Border_Size"]);

        public Thickness Widget_BorderSize_Thickness
            => new Thickness((double)Application.Current.Resources["Widget_Border_Size"]);

        public CornerRadius Widget_Border_CornerRadius
            => new CornerRadius((double)Application.Current.Resources["Widget_Border_CornerRadius"]);

        public double Widget_Border_CornerRadiusSize
            => (double)Application.Current.Resources["Widget_Border_CornerRadius"];

        public double Widget_Border_Size =>
            (double)Application.Current.Resources["Widget_Border_Size"];

        public RectangleGeometry Widget_Border_Clip
            => new RectangleGeometry(new Rect(new Size(Widget_Border_CornerRadiusSize, Widget_Border_CornerRadiusSize)));

        public Thickness Widget_Border_LeftMargin
            => Widget_Border_CornerRadiusSize == 0 ?
            new Thickness(0, -Widget_Border_Size, 0, -Widget_Border_Size)
            : new Thickness(0, Widget_Border_CornerRadiusSize - Widget_Border_Size, 0, Widget_Border_CornerRadiusSize - Widget_Border_Size);
        
        public Thickness Widget_Border_RightMargin
            => Widget_Border_CornerRadiusSize == 0 ?
            new Thickness(0, 0, 0, -Widget_Border_Size)
            : new Thickness(0, Widget_Border_CornerRadiusSize - Widget_Border_Size, 0, Widget_Border_CornerRadiusSize - Widget_Border_Size);

        public Thickness Widget_Border_TopMargin
            =>
            Widget_Border_CornerRadiusSize == 0 ?
            new Thickness(0, 0, -Widget_Border_Size, 0)
            : new Thickness(Widget_Border_CornerRadiusSize - Widget_Border_Size, 0, Widget_Border_CornerRadiusSize - Widget_Border_Size, 0);
        public Thickness Widget_Border_BottomMargin
            =>
            Widget_Border_CornerRadiusSize == 0 ?
            new Thickness(0, 0, 0, 0)
            : new Thickness(Widget_Border_CornerRadiusSize - Widget_Border_Size, 0, Widget_Border_CornerRadiusSize - Widget_Border_Size, 0);

        #endregion

        string _title = "default widget title";
        /// <summary>
        /// widget title shown in title bar
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        bool _hasSettings = true;
        /// <summary>
        /// if true the widgets has editable settings
        /// </summary>
        public bool HasSettings
        {
            get
            {
                return _hasSettings;
            }
            set
            {
                _hasSettings = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ButtonSettingsVisibility => HasSettings ? Visibility.Visible : Visibility.Collapsed;

        bool _autoSizeToFitPanelSize = false;
        /// <summary>
        /// if true, widget size fit panel size according to panel orientation
        /// </summary>
        public bool AutoSizeToFitPanelSize
        {
            get
            {
                return _autoSizeToFitPanelSize;
            }
            set
            {
                _autoSizeToFitPanelSize = value;
                SetAutoSizeStrategy();
                NotifyPropertyChanged();
            }
        }

        string[] _members = new string[] {
            nameof(Title),
            nameof(HasSettings)
        };

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValues(this, _members);
        }

        public WidgetBaseViewModel(SerializationInfo info, StreamingContext context)
        {
            info.GetValues(this, _members);
            View = new WidgetControl(this,AutoSizableElementViewModel);
            Initialize();
        }

        public WidgetBaseViewModel(WidgetControl widget)
        {
            AutoSizableElementViewModel = new AutoSizableElementViewModel();
            View = widget;
            Initialize();
        }

        void Initialize()
        {
            View.Loaded += OnLoadedInit;
        }

        void OnLoadedInit(object o, EventArgs e)
        {
            SetAutoSizeStrategy();
        }

        void SetAutoSizeStrategy()
        {
            return;
        }
    }
}
