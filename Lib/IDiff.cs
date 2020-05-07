namespace DesktopPanelTool.Lib
{
    /// <summary>
    /// interface of an object than can perform diff
    /// </summary>
    public interface IDiff<T>
    {
        /// <summary>
        /// check if this has same properties than obj
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ObjectPropertiesDiff Compare(T obj);
    }
}
