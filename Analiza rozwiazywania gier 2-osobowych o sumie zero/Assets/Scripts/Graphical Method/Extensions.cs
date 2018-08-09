using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicalMethod
{
    public static class Extensions
    {
        private const double Epsilon = 1e-10;

        public static bool IsZero(this double d)
        {
            return Math.Abs(d) < Epsilon;
        }
    }
}
