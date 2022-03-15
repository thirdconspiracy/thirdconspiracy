using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class LetterCombinationsOfAPhoneNumber
{
    [TestCase("23", new[] { "ad", "ae", "af", "bd", "be", "bf", "cd", "ce", "cf" })]
    [TestCase("27", new[] { "ap", "aq", "ar", "as", "bp", "bq", "br", "bs", "cp", "cq", "cr", "cs" })]
    public void TestCases(string digits, string[] expected)
    {
        var actual = LetterCombinations(digits).ToArray();
        Array.Sort(actual);
        CollectionAssert.AreEquivalent(expected, actual);
    }

    public IList<string> LetterCombinations(string digits)
    {
        if (digits.Length == 0)
            return new List<string>();

        var dict = new Dictionary<char, string>
        {
            ['2'] = "abc",
            ['3'] = "def",
            ['4'] = "ghi",
            ['5'] = "jkl",
            ['6'] = "mno",
            ['7'] = "pqrs",
            ['8'] = "tuv",
            ['9'] = "wxyz"
        };

        //Brute Force
        //var result = FromLoop(digits, dict);

        //BFS
        var result = FromQueue(digits, dict);
        
        //DFS
        //var result = new List<string>();
        //BackTrack(digits, dict, result, new List<char>(), 0);
        
        return result;
    }

    private IList<string> FromQueue(string digits, Dictionary<char, string> mappings)
    {
        var q = new Queue<string>();
        q.Enqueue("");
        for (var i = 0; i < digits.Length; i++)
        {
            var opts = mappings[digits[i]];
            while (q.Peek().Length == i)
            {
                var s = q.Dequeue();
                for (var j = 0; j < opts.Length; j++)
                {
                    q.Enqueue(s+opts[j]);
                }
            }
        }

        return q.ToList();
    }

    private void BackTrack(string digits, Dictionary<char, string> dict, List<string> result, List<char> temp, int start)
    {
        if (temp.Count == digits.Length)
            result.Add(string.Join("", temp));

        for (var i = start; i < digits.Length; i++)
        {
            var allowed = dict[digits[i]];
            for (var j = 0; j < allowed.Length; j++)
            {
                temp.Add(allowed[j]);
                BackTrack(digits, dict, result, temp, i + 1);
                temp.RemoveAt(temp.Count - 1);
            }
        }
    }

    public IList<string> FromLoop(string digits, Dictionary<char, string> dict)
    {
        
        int i;
        var length = 1;
        for (i = 0; i < digits.Length; i++)
            length *= digits[i] is '7' or '9' ? 4 : 3;

        var comb = new List<string>();
        for (i = 0; i < length; i++)
        {
            var val = GetCombination(digits, i, length, dict);
            comb.Add(val);
        }

        return comb;
    }


    private string GetCombination(string digits, int index, int len, Dictionary<char, string> dict)
    {
        var sb = new StringBuilder();

        var last = 1;
        for (var i = 0; i < digits.Length; i++)
        {
            var chars = dict[digits[i]];
            last *= chars.Length;
            var div = len / last;
            var charIndex = index / div % chars.Length;
            sb.Append(chars[charIndex]);
        }

        return sb.ToString();
    }
}