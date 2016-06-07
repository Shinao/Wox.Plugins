namespace Need
{
    public static class StringHelper
    {
        public static string TruncateWithEllipsis(string s, int length)
        {
            const string ellipsis = "...";

            if (ellipsis.Length > length)
                return s;

            if (s.Length > length)
                return s.Substring(0, length - ellipsis.Length) + ellipsis;

            return s;
        }
    }
}