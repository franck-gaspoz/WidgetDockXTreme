using System.Collections.Generic;

namespace DesktopPanelTool.Lib
{
    public static class ObjectRegistry
    {
        static int _objectId = 0;
        static Dictionary<int, object> _objectById = new Dictionary<int, object>();

        public static int RegisterObject(object o)
        {
            _objectId++;
            _objectById.Add(_objectId, o);
            return _objectId;
        }

        public static object GetObject(int id)
        {
            if (_objectById.TryGetValue(id, out var o))
                return o;
            return null;
        }
    }
}
