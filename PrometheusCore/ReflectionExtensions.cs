namespace System.Reflection
{
    public static class ReflectionExtensions
    {
        public static bool TryGetAttribute<T>(this PropertyInfo propertyInfo, out T t) where T : Attribute
        {
            t = propertyInfo.GetCustomAttribute<T>();
            return t != null;
        }

    }
}
