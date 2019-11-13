using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class NumericExtensions
    {

        public static decimal RoundMoney(this decimal number)
        {
            return Math.Round(number, 2, MidpointRounding.AwayFromZero);
        }

    }
}
