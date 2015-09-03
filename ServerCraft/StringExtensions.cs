namespace ServerCraft
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get the last x characters from a string.
        /// </summary>
        /// <param name="source">Original string</param>
        /// <param name="tailLength">Number of characters to return.</param>
        /// <returns></returns>
        public static string Tail(this string source, int tailLength)
        {
            return tailLength >= source.Length ? source : source.Substring(source.Length - tailLength);
        }
    }
}