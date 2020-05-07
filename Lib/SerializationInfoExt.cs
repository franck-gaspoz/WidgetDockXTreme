//#define dbg

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace DesktopPanelTool.Lib
{
    public static class SerializationInfoExt
    {
        public static T GetValue<T>(this SerializationInfo info,string pname) {
            return (T)info.GetValue(pname, typeof(T));
        }

        public static ObjectInfos AddValues(this SerializationInfo info,object o,string[] members)
        {
            if (o == null) return new ObjectInfos(o, new List<(PropertyInfo propertyInfo, object propertyValue)>(), new List<(FieldInfo fieldInfo, object fieldValue)>());
            var t = o.GetType();

            var propertyValues
                = new List<(PropertyInfo propertyInfo, object propertyValue)>();
            var fieldValues
                = new List<(FieldInfo fieldInfo, object fieldValue)>();

            foreach (var member in members)
            {
                object value = null;
                var propinf = t.GetProperty(member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                if (propinf != null)
                {
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"get property: {member}={value} [{propinf.PropertyType.FullName}] ({t.FullName})");
#endif
                    value = propinf.GetValue(o);
                    propertyValues.Add((propinf, value));
                }
                var fieldinf = t.GetField(member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                if (fieldinf != null)
                {
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"get field: {member}={value} [{fieldinf.FieldType.FullName}] ({t.FullName})");
#endif
                    value = fieldinf.GetValue(o);
                    fieldValues.Add((fieldinf, value));
                }
                info.AddValue(member, value);
            }
            return new ObjectInfos(o, propertyValues, fieldValues);
        }

        public static ObjectInfos GetValues(this SerializationInfo info,object o,string[] members)
        {
            if (o == null) return new ObjectInfos(o,new List<(PropertyInfo propertyInfo, object propertyValue)>(),new List<(FieldInfo fieldInfo, object fieldValue)>());
            var t = o.GetType();

            var propertyValues
                = new List<(PropertyInfo propertyInfo, object propertyValue)>();
            var fieldValues 
                = new List<(FieldInfo fieldInfo, object fieldValue)>();

            foreach (var member in members)
            {
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"getValue: {member} ({t.FullName})");
#endif
                var propinf = t.GetProperty(member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                if (propinf != null && propinf.CanWrite)
                {
                    var value = info.GetValue(member, propinf.PropertyType);
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"-- property: ={value} [{propinf.PropertyType.FullName}] ({t.FullName})");
#endif
                    propinf.SetValue(o, value);
                    propertyValues.Add((propinf, value));
                }
                var fieldinf = t.GetField(member, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                if (fieldinf != null && !fieldinf.IsInitOnly)
                {
                    var value = info.GetValue(member, fieldinf.FieldType);
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"-- field: ={value} [{fieldinf.FieldType}] ({t.FullName})");
#endif
                    fieldinf.SetValue(o, value);
                    fieldValues.Add((fieldinf, value));
                }                
            }
            return new ObjectInfos(o, propertyValues, fieldValues);
        }
    }
}
