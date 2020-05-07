namespace DesktopPanelTool.Lib
{
    /// <summary>
    /// interface of an object that has key and than can perform diff
    /// </summary>
    public interface IKDiff<T>
        : IDiff<T>
    {
        /// <summary>
        /// returns an unique key for the object in the comparison context       
        /// <para>mandatory not null</para>
        /// </summary>
        object CompareKey { get; }
    }
}

