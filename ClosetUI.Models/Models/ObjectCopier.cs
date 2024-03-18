using System.Reflection;

namespace ClosetUI.Models.Models;

public static class ObjectCopier
{
    public static void CopyAllPropertiesTo<T>(this T fromObj, T toObj)
    {
        PropertyInfo[] toObjectProperties = toObj.GetType().GetProperties();
        foreach (PropertyInfo propTo in toObjectProperties)
        {
            PropertyInfo propFrom = fromObj.GetType().GetProperty(propTo.Name);
            if (propFrom != null && propFrom.CanWrite)
                propTo.SetValue(toObj, propFrom.GetValue(fromObj, null), null);
        }
    }
}
