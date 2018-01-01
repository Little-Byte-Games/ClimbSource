using System.Linq;

namespace Climb.Extensions
{
    public static class IntExtensions
    {
        public static string ToRomainNumeral(this int number)
        {
            var romanNumerals = new[]
            {
                new[] {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // ones
                new[] {"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // tens
                new[] {"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // hundreds
                new[] {"", "M", "MM", "MMM"} // thousands
            };

            var intArr = number.ToString().Reverse().ToArray();
            var len = intArr.Length;
            var romanNumeral = "";
            var i = len;

            while(i-- > 0)
            {
                romanNumeral += romanNumerals[i][int.Parse(intArr[i].ToString())];
            }

            return romanNumeral;
        }
    }
}