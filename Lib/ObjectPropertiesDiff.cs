using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopPanelTool.Lib
{
    public class ObjectPropertiesDiff
    {
        public bool HasDiff => _Diffs.Count > 0;
        public IReadOnlyList<ObjectPropertyDiff> Diffs =>
            _Diffs;
        List<ObjectPropertyDiff> _Diffs = new List<ObjectPropertyDiff>();

        public bool Check(
            object obj,
            string propertyName,
            object oldValue,
            object newValue
            )
        {
            //if (oldValue?.ToString() != newValue?.ToString())
            if ((oldValue == null && newValue == null)
                || !oldValue.Equals(newValue))
            {
                _Diffs.Add(new ObjectPropertyDiff(obj, propertyName, oldValue, newValue));
                return true;
            }
            return false;   // no diff
        }

        public void Merge(string propertyName, ObjectPropertiesDiff diffs)
        {
            foreach (var diff in diffs.Diffs)
                _Diffs.Add(
                    new ObjectPropertyDiff(
                        diff.Obj,
                        propertyName + "." + diff.PropertyName,
                        diff.OldValue,
                        diff.NewValue,
                        diff.IsAdded,
                        diff.IsRemoved
                        )
                    );
        }

        public void Check<T>
            (string propertyName,
              IEnumerable<IKDiff<T>> oldValues,
              IEnumerable<IKDiff<T>> newValues
            )
        {
            var removed = new List<IKDiff<T>>();
            var added = new List<IKDiff<T>>();
            var cmpOldValues = new List<IKDiff<T>>(oldValues);
            foreach (var oVal in oldValues)
                if (newValues
                    .Where(x => x.CompareKey.Equals(oVal.CompareKey))
                    .Count() == 0)
                {
                    removed.Add(oVal);
                    cmpOldValues.Remove(oVal);
                }
            foreach (var nVal in newValues)
                if (oldValues
                    .Where(x => x.CompareKey.Equals(nVal.CompareKey))
                    .Count() == 0)
                    added.Add(nVal);
            foreach (var o in removed)
                _Diffs.Add(new ObjectPropertyDiff(oldValues, propertyName, o, null, false, true));
            foreach (var o in added)
                _Diffs.Add(new ObjectPropertyDiff(newValues, propertyName, null, o, true, false));
            foreach (var o in cmpOldValues)
            {
                var n = newValues.Where(x => x.CompareKey.Equals(o.CompareKey)).First();
                Merge(propertyName, o.Compare((T)n));
            }
        }

        public bool HasPropertyDiff(string propertyName)
        {
            foreach (var d in _Diffs)
                if (d.PropertyName == propertyName)
                    return true;
            return false;
        }

        public IList<ObjectPropertyDiff> GetDiffs(string propertyName)
        {
            var r = new List<ObjectPropertyDiff>();
            foreach (var d in _Diffs)
                if (d.PropertyName == propertyName)
                    r.Add(d);
            return r;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var diff in _Diffs)
                sb.AppendLine(diff.ToString());
            return sb.ToString();
        }
    }
}
