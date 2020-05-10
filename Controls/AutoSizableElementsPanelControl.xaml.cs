#define dbg

using DesktopPanelTool.Animations;
using DesktopPanelTool.ComponentModels;
using DesktopPanelTool.Lib;
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
        #region attributes

        public Orientation Orientation { get; protected set; } = Orientation.Horizontal;

        readonly List<IAutoSizableElement> _elements = new List<IAutoSizableElement>();
        readonly List<(IAutoSizableElement element, Grid cell)> _elementsCells = new List<(IAutoSizableElement element, Grid cell)>();
        readonly List<GridSplitter> _splitters = new List<GridSplitter>();
        readonly List<(IAutoSizableElement element,int index)> _deferredElements = new List<(IAutoSizableElement,int)>();

        double _splitterWidth = 5;
        double _splitterHeight = 5;
        bool _initialized = false;
        Window _window;

        public double ElementSpacing
        {
            get { return (double)GetValue(ElementSpacingProperty); }
            set { SetValue(ElementSpacingProperty, value); }
        }

        public static readonly DependencyProperty ElementSpacingProperty =
            DependencyProperty.Register("ElementSpacing", typeof(double), typeof(AutoSizableElementsPanelControl), new PropertyMetadata(8d));

        public string VerticalGridSplitterStyleName
        {
            get { return (string)GetValue(VerticalGridSplitterStyleNameProperty); }
            set { SetValue(VerticalGridSplitterStyleNameProperty, value); }
        }

        public static readonly DependencyProperty VerticalGridSplitterStyleNameProperty =
            DependencyProperty.Register("VerticalGridSplitterStyleName", typeof(string), typeof(AutoSizableElementsPanelControl), new PropertyMetadata(null));

        public string HorizontalGridSplitterStyleName
        {
            get { return (string)GetValue(HorizontalGridSplitterStyleNameProperty); }
            set { SetValue(HorizontalGridSplitterStyleNameProperty, value); }
        }

        public static readonly DependencyProperty HorizontalGridSplitterStyleNameProperty =
            DependencyProperty.Register("HorizontalGridSplitterStyleName", typeof(string), typeof(AutoSizableElementsPanelControl), new PropertyMetadata(null));

        #endregion

        public AutoSizableElementsPanelControl()
        {
            InitializeComponent();
            InsertEmptyCell(0, 0);
            Loaded += AutoSizableElementsPanelControl_Loaded;
        }

        private void AutoSizableElementsPanelControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!_initialized)
            {
                _initialized = true;
                foreach (var (element, index) in _deferredElements)
                    AddElement(element, index);
                _deferredElements.Clear();
                _window = WPFHelper.FindLogicalParent<Window>(this);
                _window.SizeChanged += Window_SizeChanged;
                SetOrientation(GetOrientation());
            }
        }

        Orientation GetOrientation() => _window.ActualWidth >= _window.ActualHeight ? Orientation.Horizontal : Orientation.Vertical;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetOrientation(GetOrientation());
        }

        internal void AddElement(IAutoSizableElement element, int index=-1)
        {
            if (!Container.IsLoaded)
                _deferredElements.Add((element, index));
            else
            {
                var idx = index == -1 ? _elements.Count : index;
                element.AutoSizableElementViewModel.Index = idx;
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
                        ApplyGridCellWidthStrategy(Column(i),maximizableCount,props.WidthSizeMode,FixedMinWidth(props.MinWidth),props.Width, _splitters[i]);
                        break;
                    case Orientation.Vertical:
                        maximizableCount = _elements.Where(x => IsMaximizableSize(x.AutoSizableElementViewModel.HeightSizeMode)).Count();
                        ApplyGridCellHeightStrategy(Row(i),maximizableCount, props.WidthSizeMode, FixedMinHeight(props.MinHeight), props.Height, _splitters[i]);
                        break;
                }
            }
        }

        internal void SetOrientation(Orientation orientation)
        {
            if (orientation == Orientation) return;
            Orientation = orientation;
            var lst = _elements.ToList();
            lst.Sort(new Comparison<IAutoSizableElement>((x, y) => x.AutoSizableElementViewModel.Index.CompareTo(y.AutoSizableElementViewModel.Index)));
            Container.Children.Clear();
            Container.ColumnDefinitions.Clear();
            Container.RowDefinitions.Clear();
            _elements.Clear();
            _splitters.Clear();
            foreach (var element in lst)
            {
                var ecell = _elementsCells.Where(x => x.element == element).First();
                ecell.cell.Children.Remove((UIElement)element);
            }
            _elementsCells.Clear();
            InsertEmptyCell(0, 0);
            foreach (var element in lst)
                AddElement(element);
        }

        void SetBoundsLimits(IAutoSizableElement element)
        {
            if (element.AutoSizableElementViewModel.MinWidth != 0) return;
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

        void SetGridSplitterStyle(GridSplitter gs,Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Horizontal:
                    gs.SetResourceReference(FrameworkElement.StyleProperty, VerticalGridSplitterStyleName);
                    break;
                case Orientation.Vertical:
                    gs.SetResourceReference(FrameworkElement.StyleProperty, HorizontalGridSplitterStyleName);
                    break;
            }
            var animations = new FrameworkElementFadeInOutAnimation();
            gs.MouseEnter += (o, e) => animations.Start(gs, FrameworkElementFadeInOutAnimation.FadeInAnimationName);
            gs.MouseLeave += (o, e) => animations.Start(gs, FrameworkElementFadeInOutAnimation.FadeOutAnimationName);
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
            SetGridSplitterStyle(gs,Orientation);
            Grid.SetZIndex(gs, 1);
            gs.DragDelta += WidgetSeparatorSplitter_DragDelta;
            return gs;
        }

        void WidgetSeparatorSplitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
#if false && dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"splitter dragging dx={e.HorizontalChange},DynamicResourceExtension={e.VerticalChange}");
#endif
            var gs = (GridSplitter)sender;
            var grIndex = _splitters.IndexOf(gs);
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

        void WidgetCellContentSplitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var gs = (GridSplitter)sender;
            var cellgrid = WPFHelper.FindLogicalParent<Grid>(gs);
            var widget = (WidgetControl)cellgrid.Children[0];
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    IncreaseCellHeight(cellgrid,widget,e.VerticalChange);
                    break;
                case Orientation.Vertical:
                    IncreaseCellWidth(cellgrid,widget,e.HorizontalChange);
                    break;
            }
            e.Handled = true;
        }

        void IncreaseCellWidth(Grid grid,IAutoSizableElement element,double delta)
        {
            var cd = Column(grid,0);
            var nw = cd.ActualWidth + delta;
            nw = Math.Max(FixedMinWidth(element.AutoSizableElementViewModel.MinWidth), nw);
            cd.Width = new GridLength(nw);
        }

        void IncreaseCellHeight(Grid grid, IAutoSizableElement element, double delta)
        {
            var cd = Row(grid,0);
            var nh = cd.ActualHeight + delta;
            nh = Math.Max(FixedMinHeight(element.AutoSizableElementViewModel.MinHeight), nh);
            cd.Height = new GridLength(nh);
        }

        void IncreaseCellWidth(int i,double delta)
        {
            var cd = Column(i);
            var element = _elements[i];
            var nw = cd.ActualWidth + delta;
            nw = Math.Max(FixedMinWidth(_elements[i].AutoSizableElementViewModel.MinWidth),nw);
            cd.Width = new GridLength(nw);
        }

        void IncreaseCellHeight(int i,double delta)
        {
            var cd = Row(i);
            var element = _elements[i];
            var nh = cd.ActualHeight + delta;
            nh = Math.Max(FixedMinHeight(_elements[i].AutoSizableElementViewModel.MinHeight), nh);
            cd.Height = new GridLength(nh);
        }

        Grid BuildCell(IAutoSizableElement element)
        {
            var props = element.AutoSizableElementViewModel;
            var grid = new Grid();
            var gridSplitter = new GridSplitter();
            grid.Children.Add((UIElement)element);
            grid.Children.Add(gridSplitter);
            _elementsCells.Add((element, grid));
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    var rd = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
                    grid.RowDefinitions.Add(rd);
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    if (ElementSpacing>0)
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(ElementSpacing) });

                    gridSplitter.Height = _splitterHeight;
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Bottom;
                    gridSplitter.Margin = new Thickness(0);

                    SetGridSplitterStyle(gridSplitter, Orientation.Vertical);
                    ApplyGridCellHeightStrategy(rd, IsMaximizableSize(props.HeightSizeMode) ? 1 : 0, props.HeightSizeMode, FixedMinHeight(props.MinHeight), props.Height, gridSplitter);
                    Grid.SetRow(gridSplitter, 0);
                    break;

                case Orientation.Vertical:
                    var cd = new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Star) };
                    grid.ColumnDefinitions.Add(cd);
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0) });
                    grid.RowDefinitions.Add(new RowDefinition());
                    if (ElementSpacing>0)
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(ElementSpacing) });

                    gridSplitter.Width = _splitterWidth;
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Right;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                    gridSplitter.Margin = new Thickness(0);

                    SetGridSplitterStyle(gridSplitter, Orientation.Horizontal);
                    ApplyGridCellWidthStrategy(cd, IsMaximizableSize(props.HeightSizeMode) ? 1 : 0, props.HeightSizeMode, FixedMinHeight(props.MinWidth), props.Width, gridSplitter);
                    Grid.SetColumn(gridSplitter, 0);
                    break;
            }
            gridSplitter.DragDelta += WidgetCellContentSplitter_DragDelta;
            return grid;
        }

        double FixedMinWidth(double w) => Orientation == Orientation.Horizontal ? w+ElementSpacing : w;
        double FixedMinHeight(double h) => Orientation == Orientation.Vertical ? h+ElementSpacing : h;
        internal List<IAutoSizableElement> Elements => _elements.ToList();
        internal int IndexOf(IAutoSizableElement element) => _elements.IndexOf(element);
        ColumnDefinition Column(Grid grid,int index) => grid.ColumnDefinitions[index];
        ColumnDefinition Column(int index) => Container.ColumnDefinitions[index];
        ColumnDefinition GlueColumn => Container.ColumnDefinitions[^1];
        RowDefinition Row(Grid grid,int index) => grid.RowDefinitions[index];
        RowDefinition Row(int index) => Container.RowDefinitions[index];
        RowDefinition GlueRow => Container.RowDefinitions[^1];
        bool IsAutoSize(SizeMode sizeMode) => sizeMode == SizeMode.Auto || sizeMode == SizeMode.AutoResizable;
        bool IsSpecifiedSize(SizeMode sizeMode) => sizeMode == SizeMode.Fixed || sizeMode == SizeMode.FixedResizable || sizeMode == SizeMode.Auto || sizeMode == SizeMode.AutoResizable;
        bool IsMaximizableSize(SizeMode sizeMode) => sizeMode == SizeMode.MaximizedResizable || sizeMode == SizeMode.Maximized;
        bool IsResizableSize(SizeMode sizeMode) => sizeMode == SizeMode.MaximizedResizable || sizeMode == SizeMode.FixedResizable || sizeMode == SizeMode.AutoResizable;
    }
}
