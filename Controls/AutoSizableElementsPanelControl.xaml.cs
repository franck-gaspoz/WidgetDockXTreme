﻿#define dbg

using DesktopPanelTool.ComponentModels;
using DesktopPanelTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DesktopPanelTool.Controls
{
    public partial class AutoSizableElementsPanelControl : UserControl
    {
        public Orientation Orientation { get; protected set; } = Orientation.Horizontal;

        readonly List<IAutoSizableElement> _elements = new List<IAutoSizableElement>();
        readonly List<GridSplitter> _splitters = new List<GridSplitter>();
        readonly List<(IAutoSizableElement element,int index)> _deferredElements = new List<(IAutoSizableElement,int)>();

        double _splitterWidth = 3;
        double _splitterHeight = 3;
        double _elementSpacing = 8;

        public AutoSizableElementsPanelControl()
        {
            InitializeComponent();
            InsertEmptyCell(0,0);
            Loaded += AutoSizableElementsPanelControl_Loaded;
        }

        private void AutoSizableElementsPanelControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var (element, index) in _deferredElements)
                AddElement(element, index);
            _deferredElements.Clear();
            SetOrientation(Container.ActualWidth >= Container.ActualHeight ? Orientation.Horizontal : Orientation.Vertical);
        }

        internal void AddElement(IAutoSizableElement element, int index=-1)
        {
            if (!Container.IsLoaded)
                _deferredElements.Add((element, index));
            else
            {
                var idx = index == -1 ? _elements.Count : index;
                _elements.Insert(idx, element);
                InsertElement(element, idx);
            }
        }

        internal void RemoveElement(IAutoSizableElement element)
        {

        }

        void InsertElement(IAutoSizableElement element,int index)
        {
            var grIndex = index;
            SetBoundsLimits(element);
            var elementCell = BuildCell(element);
            InsertElementCell(element,elementCell,grIndex);
            var gsEnd = BuildGridSplitter(true);
            AddSplitterCell(gsEnd, grIndex);
            ApplyGridCellsSizingStrategy();
        }

        void RecalcMaximizedCellsSize()
        {
            int i = 0, nb = 0;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var cds = Container.ColumnDefinitions.ToList();
                    foreach (var cd in cds)
                        if (i++ < _elements.Count)
                            if (IsMaximizableSize(_elements[i - 1].AutoSizableElementViewModel.WidthSizeMode)
                                && cd.Width.GridUnitType == GridUnitType.Star)
                                nb++;
                    i = 0;
                    foreach (var cd in cds)
                        if (i++ < _elements.Count)
                            if (IsMaximizableSize(_elements[i - 1].AutoSizableElementViewModel.WidthSizeMode)
                                && cd.Width.GridUnitType == GridUnitType.Star)
                            {
                                var psize = (100 / nb);
                                cd.Width = new GridLength(psize, GridUnitType.Star);
                            }
                    GlueColumn.Width = new GridLength(0);
                    break;
                case Orientation.Vertical:
                    var rds = Container.RowDefinitions.ToList();
                    foreach (var rd in rds)
                        if (i++ < _elements.Count)
                            if (IsMaximizableSize(_elements[i - 1].AutoSizableElementViewModel.HeightSizeMode)
                                && rd.Height.GridUnitType == GridUnitType.Star)
                                nb++;
                    i = 0;
                    foreach (var rd in rds)
                        if (i++ < _elements.Count)
                            if (IsMaximizableSize(_elements[i - 1].AutoSizableElementViewModel.HeightSizeMode)
                                && rd.Height.GridUnitType == GridUnitType.Star)
                            {
                                var psize = (100 / nb);
                                rd.Height = new GridLength(psize, GridUnitType.Star);
                            }
                    GlueRow.Height = new GridLength(0);
                    break;
            }
#if false && dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"glue width = {GlueColumn.Width}");
#endif
        }

        void ApplyGridCellWidthStrategy(ColumnDefinition cd,int maximizableCount, SizeMode widthSizeMode, double minWidth, double width, GridSplitter splitter)
        {
            if (IsSpecifiedSize(widthSizeMode))
            {
                var w = IsAutoSize(widthSizeMode) ? minWidth : width;
                cd.Width = new GridLength(w);
                if (!IsResizableSize(widthSizeMode))
                    cd.MaxWidth = w;
            }

            cd.MinWidth = minWidth;

            if (IsMaximizableSize(widthSizeMode) && maximizableCount>0)
            {
                var psize = (100 / maximizableCount);
                cd.Width = new GridLength(psize, GridUnitType.Star);
            }

            if (!IsResizableSize(widthSizeMode))
                splitter.IsEnabled = false;
        }

        void ApplyGridCellHeightStrategy(RowDefinition rd, int maximizableCount,SizeMode heightSizeMode, double minHeight, double height, GridSplitter splitter)
        {
            if (IsSpecifiedSize(heightSizeMode))
            {
                var w = IsAutoSize(heightSizeMode) ? minHeight : height;
                rd.Height = new GridLength(w);
                if (!IsResizableSize(heightSizeMode))
                    rd.MaxHeight = w;
            }

            rd.MinHeight = minHeight;

            if (IsMaximizableSize(heightSizeMode) && maximizableCount>0)
            {
                var psize = (100 / maximizableCount);
                rd.Height = new GridLength(psize, GridUnitType.Star);
            }

            if (!IsResizableSize(heightSizeMode))
                splitter.IsEnabled = false;
        }

        void ApplyGridCellsSizingStrategy()
        {
            int maximizableCount = 0;
            for (int i = 0; i < _elements.Count; i++)
            {
                var props = _elements[i].AutoSizableElementViewModel;
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        maximizableCount = _elements.Where(x => IsMaximizableSize(x.AutoSizableElementViewModel.WidthSizeMode)).Count();
                        ApplyGridCellWidthStrategy(Column(i),maximizableCount,props.WidthSizeMode,props.MinWidth,props.Width, _splitters[i]);
                        break;
                    case Orientation.Vertical:
                        maximizableCount = _elements.Where(x => IsMaximizableSize(x.AutoSizableElementViewModel.HeightSizeMode)).Count();
                        ApplyGridCellHeightStrategy(Row(i),maximizableCount, props.HeightSizeMode, props.MinHeight, props.Height, _splitters[i]);
                        break;
                }
            }
        }

        internal void SetOrientation(Orientation orientation)
        {
            if (orientation == Orientation) return;
        }

        void SetBoundsLimits(IAutoSizableElement element)
        {
            var o = (FrameworkElement)element;
            o.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.AutoSizableElementViewModel.MinWidth = o.DesiredSize.Width;
            element.AutoSizableElementViewModel.MinHeight = o.DesiredSize.Height;
        }

        void InsertEmptyCell(int grIndex,double size)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var cd = new ColumnDefinition();
                    Container.ColumnDefinitions.Insert(grIndex, cd);
                    if (size > -1)
                        cd.Width = new GridLength(size);
                    break;
                case Orientation.Vertical:
                    var rd = new RowDefinition();
                    Container.RowDefinitions.Insert(grIndex, rd );
                    if (size > -1)
                        rd.Height = new GridLength(size);
                    break;
            }
        }

        void AddSplitterCell(GridSplitter gridSplitter, int grIndex)
        {
            Container.Children.Add(gridSplitter);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    Grid.SetColumn(gridSplitter, grIndex);
                    break;
                case Orientation.Vertical:
                    Grid.SetRow(gridSplitter, grIndex);
                    break;
            }
            _splitters.Add(gridSplitter);
        }

        void InsertElementCell(IAutoSizableElement element,UIElement cell,int grIndex)
        {
            var o = (FrameworkElement)element;
            
            Container.Children.Add(cell);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var cd = new ColumnDefinition();
                    Container.ColumnDefinitions.Insert(grIndex, cd );
                    Grid.SetColumn(cell, grIndex);
                    break;
                case Orientation.Vertical:
                    var rd = new RowDefinition();
                    Container.RowDefinitions.Insert(grIndex, rd );
                    Grid.SetRow(cell, grIndex);
                    break;
            }
        }

        GridSplitter BuildGridSplitter(bool alignToRightBottom)
        {
            var gs = new GridSplitter() { ShowsPreview = false };
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    gs.Width = _splitterWidth;
                    gs.VerticalAlignment = VerticalAlignment.Stretch;
                    gs.HorizontalAlignment = alignToRightBottom?HorizontalAlignment.Right:HorizontalAlignment.Left;
                    break;
                case Orientation.Vertical:
                    gs.Height = _splitterHeight;
                    gs.HorizontalAlignment = HorizontalAlignment.Stretch;
                    gs.VerticalAlignment = alignToRightBottom?VerticalAlignment.Bottom:VerticalAlignment.Top;
                    break;
            }
            Grid.SetZIndex(gs, 1);
            gs.DragDelta += Gs_DragDelta;
            return gs;
        }

#if no
        void FixNotResizableCells()
        {
            int i = 0;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var cds = Container.ColumnDefinitions.ToList();
                    foreach (var cd in cds)
                        if (i++ < _elements.Count)
                            if (IsResizableSize(_elements[i - 1].AutoSizableElementViewModel.WidthSizeMode))
                                SetColumnFixedWidth(i-1,cd.ActualWidth);
                    break;
                case Orientation.Vertical:
                    var rds = Container.RowDefinitions.ToList();
                    foreach (var rd in rds)
                        if (i++ < _elements.Count)
                            if (IsResizableSize(_elements[i - 1].AutoSizableElementViewModel.HeightSizeMode))
                                SetColumnFixedHeight(i - 1, rd.ActualHeight);
                    break;
            }
        }
#endif

#if no
        void SetColumnFixedWidth(int i, double w) => Container.ColumnDefinitions[i] = new ColumnDefinition() { Width = new GridLength(w, GridUnitType.Pixel) };

        void SetColumnFixedHeight(int i,double h) => Container.RowDefinitions[i] = new RowDefinition() { Height = new GridLength(h, GridUnitType.Pixel) };
#endif

        private void Gs_DragDelta(object sender, DragDeltaEventArgs e)
        {
#if false && dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"splitter dragging dx={e.HorizontalChange},DynamicResourceExtension={e.VerticalChange}");
#endif
            var gs = (GridSplitter)sender;
            var grIndex = _splitters.IndexOf(gs);
            IAutoSizableElement nxtElem = (grIndex < _elements.Count - 1) ? _elements[grIndex + 1] : null;
            var element = _elements[grIndex];
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    IncreaseCellWidth(grIndex, e.HorizontalChange);
                    break;
                case Orientation.Vertical:
                    IncreaseCellHeight(grIndex, e.VerticalChange);
                    break;
            }
            RecalcMaximizedCellsSize();
            e.Handled = true;
        }

        void IncreaseCellWidth(int i,double delta)
        {
            var cd = Column(i);
            var element = _elements[i];
            var nw = cd.ActualWidth + delta;
            nw = Math.Max(_elements[i].AutoSizableElementViewModel.MinWidth,nw);
            cd.Width = new GridLength(nw);
        }

        void IncreaseCellHeight(int i,double delta)
        {
            var cd = Row(i);
            var element = _elements[i];
            var nh = cd.ActualHeight + delta;
            nh = Math.Max(_elements[i].AutoSizableElementViewModel.MinHeight, nh);
            cd.Height = new GridLength(nh);
        }

        Grid BuildCell(IAutoSizableElement element)
        {
            var props = element.AutoSizableElementViewModel;
            var grid = new Grid();
            var gridSplitter = new GridSplitter();
            grid.Children.Add((UIElement)element);
            grid.Children.Add(gridSplitter);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var rd = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    grid.RowDefinitions.Add(rd);
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition() {  });
                    gridSplitter.Height = _splitterHeight;
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Center;
                    ApplyGridCellHeightStrategy(rd, IsMaximizableSize(props.HeightSizeMode) ? 1 : 0, props.HeightSizeMode, props.MinHeight, props.Height, gridSplitter);
                    Grid.SetRow(gridSplitter, 1);
                    break;
                case Orientation.Vertical:
                    var cd = new ColumnDefinition() { Width = GridLength.Auto };
                    grid.ColumnDefinitions.Add(cd);
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { });
                    gridSplitter.Width = _splitterHeight;
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                    ApplyGridCellWidthStrategy(cd, IsMaximizableSize(props.WidthSizeMode) ? 1 : 0, props.WidthSizeMode, props.MinWidth, props.Width, gridSplitter);
                    Grid.SetColumn(gridSplitter, 1);
                    break;
            }
            return grid;
        }

        void SetColumnWidth(ColumnDefinition cd, double w) => cd.Width = new GridLength(w);
        void RemoveColumnWidth(ColumnDefinition cd) => cd.Width = new GridLength(1,GridUnitType.Star);
        void SetRowWidth(RowDefinition rd, double w) => rd.Height = new GridLength(w);
        void RemoveRowWidth(RowDefinition rd) => rd.Height = new GridLength(1,GridUnitType.Star);

        internal List<IAutoSizableElement> Elements => _elements.ToList();
        internal int IndexOf(IAutoSizableElement element) => _elements.IndexOf(element);
        ColumnDefinition Column(int index) => Container.ColumnDefinitions[index];
        ColumnDefinition GlueColumn => Container.ColumnDefinitions[Container.ColumnDefinitions.Count-1];
        RowDefinition Row(int index) => Container.RowDefinitions[index];
        RowDefinition GlueRow => Container.RowDefinitions[Container.RowDefinitions.Count-1];
        bool IsAutoSize(SizeMode sizeMode) => sizeMode == SizeMode.Auto || sizeMode == SizeMode.AutoResizable;
        bool IsSpecifiedSize(SizeMode sizeMode) => sizeMode == SizeMode.Fixed || sizeMode == SizeMode.FixedResizable || sizeMode == SizeMode.Auto || sizeMode == SizeMode.AutoResizable;
        bool IsMaximizableSize(SizeMode sizeMode) => sizeMode == SizeMode.MaximizedResizable || sizeMode == SizeMode.Maximized;
        bool IsResizableSize(SizeMode sizeMode) => sizeMode == SizeMode.MaximizedResizable || sizeMode == SizeMode.FixedResizable || sizeMode == SizeMode.AutoResizable;
    }
}
