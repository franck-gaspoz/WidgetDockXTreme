#define dbg

using DesktopPanelTool.ComponentModels;
using DesktopPanelTool.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        double _emptyCellSize = 8;
        double _elementSpacing = 8;

        public AutoSizableElementsPanelControl()
        {
            InitializeComponent();
            InsertEmptyCell(0,-1);
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
            var elementCell = BuildCell(element);
            InsertElementCell(element,elementCell,grIndex);
            var gsEnd = BuildGridSplitter(true);
            AddSplitterCell(gsEnd, grIndex);
            SetBoundsLimits(element);
            ApplyGridCellsSizingStrategy();
        }

        void ApplyGridCellWidthStrategy(ColumnDefinition cd,IAutoSizableElement element,GridSplitter splitter,int grIndex)
        {
            var nb = _elements.Count;
            var props = element.AutoSizableElementViewModel;

            if (IsSpecifiedSize(props.WidthSizeMode))
            {
                var w = element.AutoSizableElementViewModel.WidthSizeMode == SizeMode.Auto ? props.MinWidth : props.Width;
                Column(grIndex).Width = new GridLength(w);
                Column(grIndex).MinWidth = w;
                Column(grIndex).MaxWidth = w;
            }

            Column(grIndex).MinWidth = props.MinWidth;

            if (IsMaximizableSize(SizeMode.Maximized))
            {
                var psize = (100 / nb);
                cd.Width = new GridLength(psize, GridUnitType.Star);
            }

            if (!IsResizableSize(props.WidthSizeMode))
                splitter.IsEnabled = false;
        }

        void ApplyGridCellsSizingStrategy()
        {
            double nb = _elements.Count;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    for (int i = 0; i < nb; i++)
                        ApplyGridCellWidthStrategy(Column(i), _elements[i],_splitters[i],i);
                    break;
                case Orientation.Vertical:
                    break;
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
            return gs;
        }

        Grid BuildCell(IAutoSizableElement element)
        {
            var grid = new Grid();
            var gridSplitter = new GridSplitter();
            grid.Children.Add((UIElement)element);
            grid.Children.Add(gridSplitter);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100,GridUnitType.Star) } );
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition() {  });
                    gridSplitter.Height = _splitterHeight;
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(gridSplitter, 1);
                    break;
                case Orientation.Vertical:
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Star) });
                    gridSplitter.Width = _splitterHeight;
                    gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;
                    gridSplitter.VerticalAlignment = VerticalAlignment.Stretch; 
                    Grid.SetColumn(gridSplitter, 1);
                    break;
            }
            return grid;
        }

        internal List<IAutoSizableElement> Elements => _elements.ToList();
        internal int IndexOf(IAutoSizableElement element) => _elements.IndexOf(element);
        ColumnDefinition Column(int index) => Container.ColumnDefinitions[index];
        RowDefinition Row(int index) => Container.RowDefinitions[index];
        bool IsSpecifiedSize(SizeMode sizeMode) => sizeMode == SizeMode.Fixed || sizeMode == SizeMode.Auto;
        bool IsMaximizableSize(SizeMode sizeMode) => sizeMode == SizeMode.MaximizedResizable || sizeMode == SizeMode.Maximized;
        bool IsResizableSize(SizeMode sizeMode) => sizeMode == SizeMode.MaximizedResizable || sizeMode == SizeMode.FixedResizable || sizeMode == SizeMode.AutoResizable;
    }
}
