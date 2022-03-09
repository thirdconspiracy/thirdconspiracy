using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class StringToIntAtoi
{
    [TestCase("20000000000000000000", 2147483647)]
    [TestCase("00000000000000000002", 2)]
    [TestCase("-20000000000000000000", -2147483648)]
    [TestCase("-00000000000000000002", -2)]
    [TestCase("  -0012a42", -12)]
    [TestCase("  +  413", 0)]
    [TestCase("-91283472332", -2147483648)]
    [TestCase("-283472332", -283472332)]
    [TestCase("21474836460", 2147483647)]
    public void TestCases(string s, int expected)
    {
        var actual = MyAtoi(s);
        Assert.AreEqual(expected, actual);
    }

    private HashSet<char> d = new HashSet<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public int MyAtoi(string s)
    {
        var token = FirstToken(s);
        var cleaned = CleanToken(token);

        if (TryConvert(cleaned, out var num))
            return num;
        return 0;
    }

    private IEnumerable<char> FirstToken(string s)
    {
        var inToken = false;
        for (var i = 0; i < s.Length; i++)
        {
            if (s[i] == ' ')
            {
                if (inToken)
                    yield break;
                continue;
            }

            yield return s[i];
            inToken = true;
        }
    }

    private IEnumerable<char> CleanToken(IEnumerable<char> s)
    {
        var a = new HashSet<char>() { '.', '+', '-' };
        var isFirst = true;
        foreach (var c in s)
        {
            if (d.Contains(c))
                yield return c;
            else if (a.Contains(c))
            {
                if (isFirst)
                    yield return c;
                else
                    yield break;
            }
            else
                yield break;

            isFirst = false;
        }
    }

    private bool TryConvert(IEnumerable<char> s, out int num)
    {
        num = 0;
        var sb = new StringBuilder();
        foreach (var c in s)
        {
            sb.Append(c);
        }
        var atoi = sb.ToString();
        
        if (atoi.Length > 11)
        {
            var neg = atoi[0] == '-';
            for (var i = 0; i < atoi.Length - 11; i++)
                if (d.Contains(atoi[i]) && atoi[i] != '0')
                {
                    num = neg
                        ? int.MinValue
                        : int.MaxValue;
                    return true;
                }
            
            atoi = neg 
                ? string.Concat("-", atoi.AsSpan(atoi.Length - 11))
                : atoi.Substring(atoi.Length - 11);
        }
        Console.WriteLine(atoi);

        if (long.TryParse(atoi, out var l))
        {
            if (l > int.MaxValue)
                num = int.MaxValue;
            else if (l < int.MinValue)
                num = int.MinValue;
            else
                num = (int)l;
            return true;
        }

        return false;
    }
}