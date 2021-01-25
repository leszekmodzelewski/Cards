namespace GeoLib
{
    using System;
    using System.Globalization;

    public static class ParsingUtils
    {
        public static string FormatNullableDouble(Func<double?> getter)
        {
            object[] args = new object[] { getter() };
            return string.Format(CultureInfo.InvariantCulture, "{0:0}", args);
        }

        public static void ParseNullableDouble(string text, Action<double?> setter)
        {
            if (!string.IsNullOrEmpty(text))
            {
                ParseNullableDouble2(text, setter);
            }
            else
            {
                double? nullable = null;
                setter(nullable);
            }
        }

        public static void ParseNullableDouble2(string text, Action<double?> setter)
        {
            double num;
            if (double.TryParse(text, out num))
            {
                setter(new double?(num));
            }
        }
    }
}

