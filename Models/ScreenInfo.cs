using DesktopPanelTool.Lib;
using System;
using System.ComponentModel;

namespace DesktopPanelTool.Models
{
    [Serializable]
    public class ScreenInfo
            : ModelBase,
            IKDiff<ScreenInfo>
    {
        int _X = -1;
        /// <summary>
        /// x pos in desktop screens mosaic
        /// </summary>
        public int X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
                NotifyPropertyChanged();
            }
        }

        int _Y = -1;
        /// <summary>
        /// y pos in desktop screens mosaic
        /// </summary>
        public int Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
                NotifyPropertyChanged();
            }
        }

        int _Width = -1;
        /// <summary>
        /// screen width
        /// </summary>
        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                _Width = value;
                NotifyPropertyChanged();
            }
        }

        int _Height = -1;
        /// <summary>
        /// screen height
        /// </summary>
        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                _Height = value;
                NotifyPropertyChanged();
            }
        }

        bool _IsPrimary = false;
        public bool IsPrimary
        {
            get
            {
                return _IsPrimary;
            }
            set
            {
                _IsPrimary = value;
                NotifyPropertyChanged();
            }
        }

        public Area MonitorArea;
        public Area WorkArea;
        public string DeviceName;

        public object CompareKey => DeviceName;

        public override string ToString()
        {
            return
                $"primary={IsPrimary} X={X} Y={Y} scr.w={Width} scr.h={Height} monArea={MonitorArea} workArea={WorkArea} device={DeviceName}";
        }

        /// <summary>
        /// check if this screen has same properties (os) than the provided one
        /// </summary>
        /// <param name="screenInfo">compare with</param>
        /// <returns>check if has same properties (primary,monitor aera,physical aera,x,y)</returns>
        /// <summary>       
        /// <returns>differences decription</returns>
        public ObjectPropertiesDiff Compare(ScreenInfo screenInfo)
        {
            var diffs = new ObjectPropertiesDiff();
            diffs.Merge(nameof(WorkArea), WorkArea.Compare(screenInfo.WorkArea));
            diffs.Merge(nameof(MonitorArea), MonitorArea.Compare(screenInfo.MonitorArea));
            diffs.Check(this, nameof(Width), Width, screenInfo.Width);
            diffs.Check(this, nameof(Height), Height, screenInfo.Height);
            diffs.Check(this, nameof(IsPrimary), IsPrimary, screenInfo.IsPrimary);
            diffs.Check(this, nameof(X), X, screenInfo.X);
            diffs.Check(this, nameof(Y), Y, screenInfo.Y);
            return diffs;
        }
    }
}
