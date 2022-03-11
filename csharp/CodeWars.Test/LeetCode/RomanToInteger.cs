using System.Collections.Generic;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class RomanToInteger
{
    [TestCase(3, "III")]
    [TestCase(58, "LVIII")]
    [TestCase(1994, "MCMXCIV")]
    [TestCase(4, "IV")]
    [TestCase(9, "IX")]
    [TestCase(40, "XL")]
    [TestCase(90, "XC")]
    [TestCase(400, "CD")]
    [TestCase(900, "CM")]
    public void TestCases(int expected, string s)
    {
        var actual = RomanToInt(s);
        Assert.AreEqual(expected, actual);
    }
    
    public int RomanToInt(string s)
    {
        //return RomanToIntFixed(s);
        return RomanToIntLoop(s);
    }
    
    public int RomanToIntFixed(string s)
    {
        var dict = new Dictionary<string, int>()
        {
            ["MMM"] = 3000,
            ["MM"] = 2000,
            ["M"] = 1000,
            ["CM"] = 900,
            ["DCCC"] = 800,
            ["DCC"] = 700,
            ["DC"] = 600,
            ["D"] = 500,
            ["CD"] = 400,
            ["CCC"] = 300,
            ["CC"] = 200,
            ["C"] = 100,
            ["XC"] = 90,
            ["LXXX"] = 80,
            ["LXX"] = 70,
            ["LX"] = 60,
            ["L"] = 50,
            ["XL"] = 40,
            ["XXX"] = 30,
            ["XX"] = 20,
            ["X"] = 10,
            ["IX"] = 9,
            ["VIII"] = 8,
            ["VII"] = 7,
            ["VI"] = 6,
            ["V"] = 5,
            ["IV"] = 4,
            ["III"] = 3,
            ["II"] = 2,
            ["I"] = 1,
        };

        var total = 0;
        foreach (var kvp in dict)
        {
            if (s.StartsWith(kvp.Key))
            {
                s = s.Substring(kvp.Key.Length);
                total += kvp.Value;
            }
        }

        return total;
    }
    
    public int RomanToIntLoop(string s)
    {
        if (s.Length == 0)
            return 0;
        
        var vals = new Dictionary<char, int>
        {
            ['z'] = 0,
            ['I'] = 1,
            ['V'] = 5,
            ['X'] = 10,
            ['L'] = 50,
            ['C'] = 100,
            ['D'] = 500,
            ['M'] = 1000,
        };

        var val = vals[s[0]];
        if (s.Length == 1)
            return val;

        var startIndex = 1;
        if (val < vals[s[1]])
        {
            val = vals[s[1]] - val;
            startIndex = 2;
        }
        
        for(var i = startIndex;  i < s.Length; i++)
        {
            var cur = vals[s[i]];
            if (i < s.Length - 1 && cur < vals[s[i+1]])
                val = val - cur + vals[s[++i]];
            else
                val += cur;
        }
        
        return val;
    }
}