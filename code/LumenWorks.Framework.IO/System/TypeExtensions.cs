namespace System.Reflection
{
#if NET20 || NET35
    internal static class TypeExtensions
    {
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
    }
#endif
}