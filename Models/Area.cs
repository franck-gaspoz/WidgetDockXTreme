using DesktopPanelTool.Lib;
using System;
using static DesktopPanelTool.Models.NativeTypes;

namespace DesktopPanelTool.Models
{
    [Serializable]
    public class Area : IDiff<Area>
    {
        public object RelatedObject;

        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;

        public int Width => Math.Abs(Right - Left);
        public int Height => Math.Abs(Bottom - Top);

        public Area(object relatedObject, int left, int top, int right, int bottom)
        {
            RelatedObject = relatedObject;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static Area FromRect(object relatedObject, Rect rect)
        {
            return new Area(
                relatedObject,
                rect.Left, rect.Top, rect.Right, rect.Bottom
                );
        }

        public override string ToString()
        {
            return $"({Left},{Top})-({Right},{Bottom})[{Width}x{Height}]";
        }

        /// <summary>
        /// check if this aera has same properties (os) than the provided one
        /// </summary>
        /// <param name="Area">compare with</param>
        /// <returns>differences decription</returns>
        public ObjectPropertiesDiff Compare(Area area)
        {
            var diff = new ObjectPropertiesDiff();
            diff.Check(this, nameof(Left), Left, area.Left);
            diff.Check(this, nameof(Top), Top, area.Top);
            diff.Check(this, nameof(Right), Right, area.Right);
            diff.Check(this, nameof(Bottom), Bottom, area.Bottom);
            diff.Check(this, nameof(Width), Width, area.Width);
            diff.Check(this, nameof(Height), Height, area.Height);
            return diff;
        }

        public bool ContainsPoint(double x,double y)
        {
            return x >= Left && y >= Top && x <= Right && y <= Bottom;
        }
    }
}
