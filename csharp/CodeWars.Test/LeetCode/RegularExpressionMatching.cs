using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class RegularExpressionMatching
{
    [TestCase("aa", "a", false)]
    [TestCase("aa", "a*", true)]
    [TestCase("ab", ".*", true)]
    public void TestCase(string s, string p, bool expected)
    {
        var actual = IsArrayMatch(s, p);
        //var actual = IsRecursiveMatch(s, p);
        Assert.AreEqual(expected, actual);
    }

    private bool IsArrayMatch(string s, string p)
    {
        var arr = new bool[s.Length + 1, p.Length + 1];
        arr[0, 0] = true;
        
        //will match empty string if '*' represents 0 instances every time
        var rows = arr.GetLength(0);
        var col = arr.GetLength(1);
        
        for (var i = 1; i < col; i++)
        {
            if (p[i - 1] == '*')
                arr[0, i] = arr[0, i - 2];
        }

        for (var i = 1; i < rows; i++)
        {
            for (var j = 1; j < col; j++)
            {
                if (IsCharMatch(s[i - 1], p[j - 1]))
                    arr[i, j] = arr[i - 1, j - 1];
                else if (p[j - 1] == '*')
                {
                    arr[i, j] = arr[i, j - 2];//Zero
                    if (!arr[i,j] && IsCharMatch(s[i - 1], p[j - 2]))//Many
                        arr[i, j] = arr[i - 1, j];
                }
                else
                    arr[i, j] = false;
            }
        }

        return arr[s.Length, p.Length];
    }

    private bool IsRecursiveMatch(string s, string p)
    {
        if (p.Length == 0)
            return s.Length == 0;

        if (p.Length == 1)
            return s.Length == 1 && IsCharMatch(s[0], p[0]);

        if (p[1] == '*')
        {
            //Zero instances
            if (IsRecursiveMatch(s, p.Substring(2)))
                return true;
            //1+ instances
            return s.Length != 0
                   && IsCharMatch(s[0], p[0])
                   && IsRecursiveMatch(s.Substring(1), p);
        }

        //exact
        if (s.Length == 0)
            return false;

        if (p[0] != s[0] && p[0] != '.')
            return false;

        return IsRecursiveMatch(s.Substring(1), p.Substring(1));
    }

    private static bool IsCharMatch(char c, char p) => p == c || p == '.';
}