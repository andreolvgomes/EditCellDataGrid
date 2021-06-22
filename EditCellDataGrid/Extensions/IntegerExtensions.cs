namespace System
{
    public static class IntegerExtensions
    {
        public static Int16 CastToInt16(this string str)
        {
            Int16 result = 0;
            var res = Int16.TryParse(str, out result);
            return result;
        }
    }
}