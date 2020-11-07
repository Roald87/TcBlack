using System;
using System.Text;

namespace TcBlackCore
{
    /// <summary>
    /// Source: https://stackoverflow.com/a/47915552/6329629
    /// </summary>
    public static class StringExtensions
    {
        public static string Repeat(this string s, int n)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            string _repeatedString = new StringBuilder(s.Length * n)
                .Insert(0, s, n)
                .ToString();

            return _repeatedString;
        }
    }
}