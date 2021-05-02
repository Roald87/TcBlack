using System;
using System.Text;

namespace TcBlackCore
{
    /// <summary>
    /// Source: https://stackoverflow.com/a/47915552/6329629
    /// </summary>
    public static class StringExtensions
    {
        public static string Repeat(this string text, int times)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            string _repeatedString = new StringBuilder(text.Length * times)
                .Insert(0, text, times)
                .ToString();

            return _repeatedString;
        }
    }
}