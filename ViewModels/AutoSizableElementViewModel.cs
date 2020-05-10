using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using System;

namespace DesktopPanelTool.ViewModels
{
    [Serializable]
    public class AutoSizableElementViewModel
        : ViewModelBase
    {
        SizeMode _widthSizeMode = SizeMode.MaximizedResizable;
        /// <summary>
        /// width size mode
        /// </summary>
        public SizeMode WidthSizeMode
        {
            get
            {
                return _widthSizeMode;
            }
            set
            {
                _widthSizeMode = value;
                NotifyPropertyChanged();
            }
        }

        double _minWidth = 0;
        /// <summary>
        /// min width == measured width (size mode auto)
        /// </summary>
        public double MinWidth
        {
            get
            {
                return _minWidth;
            }
            set
            {
                _minWidth = value;
                NotifyPropertyChanged();
            }
        }

        double _minHeight = 0;
        /// <summary>
        /// min height == measured height (size mode auto)
        /// </summary>
        public double MinHeight
        {
            get
            {
                return _minHeight;
            }
            set
            {
                _minHeight = value;
                NotifyPropertyChanged();
            }
        }

        double _width = 48;
        /// <summary>
        /// width (size mode fixed)
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                NotifyPropertyChanged();
            }
        }

        double _height = 48;
        /// <summary>
        /// height (size mode fixed)
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                NotifyPropertyChanged();
            }
        }

        public AutoSizableElementViewModel()
        {
        }
    }
}
