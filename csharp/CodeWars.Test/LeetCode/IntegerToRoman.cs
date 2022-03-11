using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class IntegerToRoman
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
    public void TestCases(int num, string expected)
    {
        var actual = IntToRoman(num);
        Assert.AreEqual(expected, actual);
    }
    
    public string IntToRoman(int num)
    {
        return IntToRomanFixed(num);
    }
    public string IntToRomanFixed(int num)
    {
        string[] ones = {"","I","II","III","IV","V","VI","VII","VIII","IX"};
        string[] tens = {"","X","XX","XXX","XL","L","LX","LXX","LXXX","XC"};
        string[] hund = {"","C","CC","CCC","CD","D","DC","DCC","DCCC","CM"};
        string[] thou = {"","M","MM","MMM"};
        
        return thou[num/1000] + hund[(num%1000)/100] + tens[(num%100)/10] + ones[num%10];
    }
    
    public string IntToRomanLoop(int num)
    {
        if (num == 0)
            return string.Empty;

        var dict = new Dictionary<int, string>()
        {
            [1000] = "M",
            [900] = "CM",
            [500] = "D",
            [400] = "CD",
            [100] = "C",
            [90] = "XC",
            [50] = "L",
            [40] = "XL",
            [10] = "X",
            [9] = "IX",
            [5] = "V",
            [4] = "IV",
            [1] = "I"
        };

        var sb = new StringBuilder();
        foreach (var kvp in dict)
        {
            var count = num / kvp.Key;
            num = num % kvp.Key;
            for (var i = 0; i < count; i++)
                sb.Append(kvp.Value);
        }

        return sb.ToString();
    }
    
}