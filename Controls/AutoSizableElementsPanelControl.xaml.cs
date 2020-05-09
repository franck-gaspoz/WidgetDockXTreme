using DesktopPanelTool.ComponentModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Controls
{
    public partial class AutoSizableElementsPanelControl : UserControl
    {
        public Orientation Orientation { get; protected set; } = Orientation.Horizontal;

        List<IAutoSizableElement> _elements = new List<IAutoSizableElement>();
        double _splitterWidth = 8;
        double _splitterHeight = 8;

        public AutoSizableElementsPanelControl()
        {
            InitializeComponent();
            Loaded += AutoSizableElementsPanelControl_Loaded;
        }

        private void AutoSizableElementsPanelControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetOrientation(Container.ActualWidth >= Container.ActualHeight ? Orientation.Horizontal : Orientation.Vertical);
        }

        internal void AddElement(IAutoSizableElement element, int index=-1)
        {
            var idx = index == -1 ? _elements.Count : index;
            InsertElement(element, idx);
            _elements.Insert(idx,element);
        }

        void InsertElement(IAutoSizableElement element,int index)
        {
            var grIndex = index <= 1 ? index : index * 2 - 1;
            var elementCell = BuildCell(element);
            if (index>0)
            {
                var gs = BuildGridSplitter();
                InsertSplitterCell(gs, grIndex);
                grIndex++;
            }
            InsertElementCell(element,elementCell, grIndex);
        }

        void InsertSplitterCell(GridSplitter gridSplitter, int grIndex)
        {
            Container.Children.Add(gridSplitter);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    Container.ColumnDefinitions.Insert(grIndex, new ColumnDefinition() { Width = GridLength.Auto });
                    Grid.SetColumn(gridSplitter, grIndex);
                    break;
                case Orientation.Vertical:
                    Grid.SetRow(gridSplitter, grIndex);
                    Container.RowDefinitions.Insert(grIndex, new RowDefinition() { Height = GridLength.Auto });
                    break;
            }
        }

        void InsertElementCell(IAutoSizableElement element,UIElement cell,int grIndex)
        {
            var o = (FrameworkElement)element;
            
            Container.Children.Add(cell);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    Container.ColumnDefinitions.Insert(grIndex, new ColumnDefinition() { 
                        //Width = new GridLength(0, GridUnitType.Star)
                    });
                    Grid.SetColumn(cell, grIndex);
                    break;
                case Orientation.Vertical:
                    Grid.SetRow(cell, grIndex);
                    Container.RowDefinitions.Insert(grIndex, 
                        new RowDefinition() { Height = new GridLength(0, GridUnitType.Star) });
                    break;
            }
        }

        GridSplitter BuildGridSplitter()
        {
            var gs = new GridSplitter() { ShowsPreview = false };
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    gs.Width = _splitterWidth;
                    gs.VerticalAlignment = VerticalAlignment.Stretch;
                    gs.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case Orientation.Vertical:
                    gs.Height = _splitterHeight;
                    gs.HorizontalAlignment = HorizontalAlignment.Stretch;
                    gs.VerticalAlignment = VerticalAlignment.Center;
                    break;
            }
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

        internal void RemoveElement(IAutoSizableElement element)
        {

        }

        internal void SetOrientation(Orientation orientation)
        {
            if (orientation == Orientation) return;
        }
    }
}
