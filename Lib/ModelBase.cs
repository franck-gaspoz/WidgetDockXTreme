using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopPanelTool.Lib
{
    [Serializable]
    public class ModelBase
        : INotifyPropertyChanged
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
