using System;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class ContainerWithMostWater
{
    [TestCase(new []{1,8,6,2,5,4,8,3,7}, 49)]
    [TestCase(new []{1,1}, 1)]
    public void TestCases(int[] height, int expected)
    {
        var actual = MaxArea(height);
        Assert.AreEqual(expected, actual);
    }
    public int MaxArea(int[] height)
    {
        if (height.Length < 2)
            return 0;

        if (height.Length == 2)
            return height[0] < height[1]
                ? height[0]
                : height[1];
        
        //return MaxAreaSquared(height);
        return MaxAreaConverge(height);
    }

    private int MaxAreaConverge(int[] height)
    {
        var left = 0;
        var right = height.Length - 1;
        var maxArea = 0;
        while (left < right)
        {
            maxArea = height[left] < height[right]
                ? Math.Max(maxArea, height[left] * (right - left++))
                : Math.Max(maxArea, height[right] * (right-- - left));
        }

        return maxArea;
    }

    private int MaxAreaSquared(int[] height)
    {
        var l = 0;
        var r = 0;
        var max = 0;
        for (var i = 0; i < height.Length; i++)
        {
            for (var j = i + 1; j < height.Length; j++)
            {
                var cur = GetVolume(height, i, j);
                if (cur > max)
                {
                    max = cur;
                }
            }
        }

        return max;
    }

    private int GetVolume(int[] height, int i, int j)
        => Math.Min(height[i], height[j]) * (j-i);
}