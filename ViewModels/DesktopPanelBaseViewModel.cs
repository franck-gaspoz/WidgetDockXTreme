//#define dbg

using DesktopPanelTool.Behaviors.WindowBehaviors;
using DesktopPanelTool.Controls;
using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using DesktopPanelTool.Services;
using DesktopPanelTool.Views;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace DesktopPanelTool.ViewModels
{
    [Serializable]
    public class DesktopPanelBaseViewModel
        : ViewModelBase,
        IDesktopPanelBaseViewModel,
        ISerializable
    {
        public DesktopPanelBase View { get; protected set; }

        public ObjectInfos SettingsBackup { get; protected set; }

        public ObservableCollection<WidgetBaseViewModel> WidgetsViewModels { get; protected set; }
            = new ObservableCollection<WidgetBaseViewModel>();

        string _title = "desktop panel default title";
        /// <summary>
        /// desktop panel title
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

        bool _isPined;
        /// <summary>
        /// indicates if the panel is pined or not 
        /// </summary>
        public bool IsPined { get { return Behavior.IsPined; } set { _isPined = value; } }

        bool _isDocked;
        /// <summary>
        /// indicates if the panel is docked or not
        /// </summary>
        public bool IsDocked { get { return Behavior.IsDocked; } set { _isDocked = value; }  }

        bool _isCollapsed;
        /// <summary>
        /// indicates if the panel is collapsed or not
        /// </summary>
        public bool IsCollapsed { get { return Behavior.IsHidden; } set { _isCollapsed = value; } } 

        DockName _dock;
        /// <summary>
        /// indicates the dock where the panel is docked
        /// </summary>
        public DockName Dock { get { return Behavior.Dock; } set { _dock = value; } }

        ScreenInfo _dockScreen;

        /// <summary>
        /// screen where panel is docked
        /// </summary>
        public ScreenInfo DockScreen { get { return Behavior.DockScreen; } set { _dockScreen = value; } }

        DockablePanelWindowBehavior Behavior
            => Interaction.GetBehaviors(View)
                .OfType<DockablePanelWindowBehavior>()
                .FirstOrDefault();

        MovableTransparentWindowBehavior MoveBehavior
            => Interaction.GetBehaviors(View)
                .OfType<MovableTransparentWindowBehavior>()
                .FirstOrDefault();

        public bool IsMoving => MoveBehavior.IsDragging;

        public void NotifyPropertyUpdated(string propName)
        {
            NotifyPropertyChanged(propName);
        }

        public void TogglePin()
        {
            Behavior.TogglePin();
        }

        public void Pin()
        {
            Behavior.Pin();
        }

        public void UnPin()
        {
            Behavior.UnPin();
        }

        public void Expand()
        {
            Behavior.ExpandPanel();
        }

        public void Collapse()
        {
            Behavior.CollapsePanel();
        }

        public void AttachToDock(
            DockName dock,
            ScreenInfo dockScreen = null)
        {
            if (dockScreen == null)
                dockScreen = DisplayDevices
                    .GetScreensInfos()
                    .Where(x => x.IsPrimary)
                    .FirstOrDefault();
            if (dockScreen != null)
                Behavior.AttachToDock(dock, dockScreen);
        }

        public void Close()
        {
            DesktopPanelToolService.CloseDockPanel(View);
        }

        public void AddWidget(WidgetControl widget,int targetIndex=-1)
        {
            WidgetsViewModels.Add(widget.ViewModel);
            widget.ViewModel.PanelViewModel = this;
            if (targetIndex == -1)
            {
                /*View.WidgetsPanel.Children.Add(GetNewWidgetStackPanelDropPlaceHolder());
                View.WidgetsPanel.Children.Add(widget);*/
                View.WidgetsPanel.AddElement(widget);
            } else
            {
                /*View.WidgetsPanel.Children.Insert(targetIndex, widget);
                View.WidgetsPanel.Children.Insert(targetIndex, GetNewWidgetStackPanelDropPlaceHolder());*/
            }
        }

        WidgetStackPanelDropPlaceHolder GetNewWidgetStackPanelDropPlaceHolder()
        {
            var viewModel = new WidgetStackPanelDropPlaceHolderViewModel(this);

            var view = new WidgetStackPanelDropPlaceHolder(viewModel);
            view.SetResourceReference(WidgetStackPanelDropPlaceHolder.DropSensitiveAreahighlightBackgroundBrushProperty,
                (string)View.FindResource("DropAreaSensitiveAreaSmallHighlightBackgroundKey"));
            return view;
        }

        public void CloseWidget(WidgetControl widget)
        {
            if (WidgetsViewModels.Remove(widget.ViewModel))
            {
                /*var index = View.WidgetsPanel.Children.IndexOf(widget);
                var dropholder = View.WidgetsPanel.Children[index - 1];
                View.WidgetsPanel.Children.Remove(widget);
                View.WidgetsPanel.Children.Remove(dropholder);*/
                var index = View.WidgetsPanel.IndexOf(widget);
                View.WidgetsPanel.RemoveElement(widget);
#if alldbg || dbg
                DumpWidgetsPanelChildren();
#endif
            }
        }

        internal void DumpWidgetsPanelChildren()
        {
            DesktopPanelTool.Lib.Debug.WriteLine($"----------- panel={Title}");
            foreach (var o in View.WidgetsPanel.Elements)
            {
                var title = "";
                if (o is WidgetControl wc)
                    title = $"[{wc.ViewModel.Title}]";
                DesktopPanelTool.Lib.Debug.WriteLine($"o={o} {title}");
            }
        }

        string[] _members = new string[] {
            nameof(WidgetsViewModels),
            nameof(Title),
            nameof(IsPined),
            nameof(IsDocked),
            nameof(IsCollapsed),
            nameof(Dock),
            nameof(DockScreen)
        };

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SettingsBackup = info.AddValues(this, _members);
        }

        public DesktopPanelBaseViewModel(SerializationInfo info, StreamingContext context)
        {
            SettingsBackup = info.GetValues(this, _members);
            View = new DesktopPanelBase(this);
            Initialize();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            var widgets = new List<WidgetBaseViewModel>(WidgetsViewModels);
            WidgetsViewModels.Clear();
            foreach (var widget in widgets)
                AddWidget(widget.View);
            DesktopPanelToolService.AddDesktopPanel(
                View,
                _dock,
                _dockScreen,
                _isPined,
                _isCollapsed
                );
        }

        public DesktopPanelBaseViewModel(DesktopPanelBase view)
        {
            View = view;
            Initialize();
        }

        void Initialize()
        {
            View.Loaded += (o, e) => InitializePermanentWidgetDropPlaceHolder();
            PropertyChanged += DesktopPanelBaseViewModel_PropertyChanged;
        }

        private void DesktopPanelBaseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName==nameof(IsDocked) && IsDocked)
            {
                View.WidgetsPanel.ResetDisposition();
            }
        }

        void InitializePermanentWidgetDropPlaceHolder()
        {
            View.PermanentWidgetDropPlaceHolder.ViewModel = new WidgetStackPanelDropPlaceHolderViewModel(this);
        }
    }
}
