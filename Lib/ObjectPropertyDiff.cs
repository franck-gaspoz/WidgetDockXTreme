namespace DesktopPanelTool.Lib
{
    public class ObjectPropertyDiff
    {
        public readonly object Obj;
        public readonly string PropertyName;
        public readonly object OldValue;
        public readonly object NewValue;
        public readonly bool IsAdded;
        public readonly bool IsRemoved;

        public ObjectPropertyDiff(
            object obj,
            string propertyName,
            object oldValue,
            object newValue,
            bool isAdded = false,
            bool isRemoved = false)
        {
            Obj = obj;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
            IsAdded = isAdded;
            IsRemoved = isRemoved;
        }

        public override string ToString()
        {
            if (IsAdded)
                return $"({Obj.GetType().Name}#{Obj.GetHashCode().ToString("X")}) [+] property={PropertyName} newVal= {NewValue}";
            if (IsRemoved)
                return $"({Obj.GetType().Name}#{Obj.GetHashCode().ToString("X")}) [-] property={PropertyName} oldVal= {OldValue}";
            return $"({Obj.GetType().Name}#{Obj.GetHashCode().ToString("X")}) [≠] property={PropertyName} oldVal={OldValue} newVal={NewValue}";
        }
    }
}
