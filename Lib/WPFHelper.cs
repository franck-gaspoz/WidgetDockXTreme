using System;
using System.Collections.Generic;
using dr=System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;


namespace DesktopPanelTool.Lib
{
    internal static class WPFHelper
    {
        /// <summary>
        /// get a drawing Icon from a WPF application resource located at given uri
        /// </summary>
        /// <param name="iconUri">icon uri</param>
        internal static dr.Icon GetIcon(
            string iconUri
            )
        {
            return new dr.Icon(
                Application.GetResourceStream(
                    new Uri(
                        iconUri,
                        UriKind.Relative)
                    ).Stream
                );
        }

        internal static T FindLogicalParent<T>(DependencyObject o)
        {
            if (o == null) return default;
            if (o is FrameworkElement p)
            {
                if (p == null) return default;
                if (p is T ot) return ot;
                return FindLogicalParent<T>(p.Parent);
            }
            return default;
        }

        internal static DependencyObject FindLogicalAncestor(DependencyObject o)
        {
            if (o == null) return default;
            if (o is FrameworkElement p)
            {
                if (p.Parent == null) return p;
                return FindLogicalAncestor(p.Parent);
            }
            return o;
        }

        internal static T FindParent<T>(DependencyObject o)
        {
            if (o == null) return default;
            var p = VisualTreeHelper.GetParent(o);
            if (p == null) return default;
            if (p is T ot) return ot;
            return FindParent<T>(VisualTreeHelper.GetParent(p));
        }

        internal static DependencyObject FindParent(Type t,DependencyObject o)
        {
            if (o == null) return default;
            var p = VisualTreeHelper.GetParent(o);
            if (p == null) return default;
            if (p.GetType()==t) return p;
            return FindParent(t,VisualTreeHelper.GetParent(p));
        }

        internal static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

        internal static List<DependencyObject> GetChilds(DependencyObject o)
        {
            var r = new List<DependencyObject>();
            if (o == null)
                return r;
            int childrenCount = VisualTreeHelper.GetChildrenCount(o);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                r.Add(child);
                r.AddRange(GetChilds(child));
            }
            return r;
        }

        internal static List<T> FindChilds<T>(DependencyObject parent,bool recurseMatchingChilds=false)
           where T : DependencyObject
        {
            var r = new List<T>();
            if (parent == null) return r;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T c)
                {
                    r.Add(c);
                    if (recurseMatchingChilds)
                        r.AddRange(FindChilds<T>(c, recurseMatchingChilds));
                }
                else
                    r.AddRange(FindChilds<T>(child, recurseMatchingChilds));
            }
            return r;
        }

        internal static List<DependencyObject> FindChilds(Type t,DependencyObject parent, bool recurseMatchingChilds = false)
        {
            var r = new List<DependencyObject>();
            if (parent == null) return r;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child.GetType() == t)
                {
                    r.Add(child);
                    if (recurseMatchingChilds)
                        r.AddRange(FindChilds(t, child, recurseMatchingChilds));
                }
                else
                    r.AddRange(FindChilds(t, child, recurseMatchingChilds));
            }
            return r;
        }

        internal static bool HasParent<T>(DependencyObject o) => FindParent<T>(o) != null;
        internal static bool HasParent(Type t,DependencyObject o) => FindParent(t,o) != null;

        internal static RenderTargetBitmap GetRenderTargetBitmap(FrameworkElement element)
        {
            var size = element.DesiredSize;
            var bm = element.Margin;
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(new Point(), size));

            RenderTargetBitmap rtb =
              new RenderTargetBitmap(
                (int)size.Width+1,
                (int)size.Height+1,
                96, 96, PixelFormats.Pbgra32);

            rtb.Render(element);

            element.Margin = bm;

            return rtb;
        }

        internal static BitmapSource GetImageMask(RenderTargetBitmap rtb)
        {
            int stride = (int)rtb.PixelWidth * (rtb.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)rtb.PixelHeight * stride];
            rtb.CopyPixels(pixels, stride, 0);
            for (int i = 0; i < pixels.Length; i += 4)
            {
                if (pixels[i+3] != 0) {
                    pixels[i] = pixels[i + 1] = pixels[i + 2] = 0;
                    pixels[i+3] = 255;
                 }
            };
            var bitmap = new WriteableBitmap((int)rtb.Width, (int)rtb.Height, 96, 96, PixelFormats.Pbgra32, null);
            bitmap.WritePixels(new Int32Rect(0, 0, (int)rtb.Width, (int)rtb.Height), pixels, (int)rtb.Width * (bitmap.Format.BitsPerPixel / 8), 0);
            return bitmap;
        }
    }
}
