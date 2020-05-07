using System.Collections.Generic;
using System.Reflection;

namespace DesktopPanelTool.Lib
{
    public class ObjectInfos
    {
        public object Obj { get; protected set; }
        public List<(PropertyInfo propertyInfo, object propertyValue)> PropertyValues { get; protected set; }
        public List<(FieldInfo fieldInfo, object fieldValue)> FieldValues { get; protected set; }

        public ObjectInfos(
            object obj, 
            List<(PropertyInfo propertyInfo, object propertyValue)> propertyValues, 
            List<(FieldInfo fieldInfo, object fieldValue)> fieldValues)        
        {
            Obj = obj;
            PropertyValues = new List<(PropertyInfo propertyInfo, object propertyValue)>(propertyValues);
            FieldValues = new List<(FieldInfo fieldInfo, object fieldValue)>(fieldValues);
        }
    }
}
