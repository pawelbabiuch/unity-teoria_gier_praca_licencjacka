using System.Linq;

namespace GameTheory
{
    static class GreatestCommonDivisor
    {
        /// <summary>
        /// Funkcja zwraca największy wspólny dzielnik dla tablicy wypłat
        /// </summary>
        /// <param name="freq">tablica wypłat</param>
        /// <returns>największy wspólny dzielnik</returns>
        public static int GetGreatestCommonDivisor(int[] freq)
        {
            return freq.Aggregate(GetGreatestCommonDivisor);
        }

        private static int GetGreatestCommonDivisor(int a, int b)
        {
            return (b == 0) ? a : GetGreatestCommonDivisor(b, a % b);
        }
    }
}
