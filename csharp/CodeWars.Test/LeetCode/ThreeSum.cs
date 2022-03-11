using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class ThreeSumTests
{
    /*
     * Given an integer array nums, return all the triplets [nums[i], nums[j], nums[k]]
     * such that:
     *   * i != j
     *   * i != k
     *   * j != k
     *   * nums[i] + nums[j] + nums[k] == 0.
     * 
     * Notice that the solution set must not contain duplicate triplets.
     */

    [TestCase(new[] { -1, 0, 1, 2, -1, -4 }, "[[ -1, -1, 2 ], [ -1, 0, 1 ]]")]
    [TestCase(new[] { 0, 0, 0 }, "[[ 0, 0, 0 ]]")]
    [TestCase(new[] { -1, 0, 1, 2, -1, -4 }, "[[-1,-1,2],[-1,0,1]]")]
    [TestCase(new[] { -2, 0, 1, 1, 2 }, "[[-2,0,2],[-2,1,1]]")]
    public void TestCases(int[] nums, string json)
    {
        var expected = JsonConvert.DeserializeObject<List<List<int>>>(json);
        var actual = ThreeSum(nums);
        CollectionAssert.AreEqual(expected, actual);
    }

    public IList<IList<int>> ThreeSum(int[] nums)
    {
        var result = new List<IList<int>>();
        if (nums == null || nums.Length < 3)
            return new List<IList<int>>();

        Array.Sort(nums);

        for (var i = 0; i < nums.Length - 2; i++)
        {
            if (nums[i] > 0)
                break;
            if (i > 0 && nums[i] == nums[i - 1])
                continue;

            var low = i + 1;
            var high = nums.Length - 1;

            while (low < high)
            {
                var sum = nums[i] + nums[low] + nums[high];
                if (sum == 0)
                {
                    result.Add(new List<int> { nums[i], nums[low], nums[high] });
                    do
                        low++;
                    while (low < high && nums[low] == nums[low - 1]);

                    do
                        high--;
                    while (low < high && nums[high] == nums[high + 1]);
                }
                else if (sum < 0)
                    low++;
                else
                    high--;
            }
        }

        return result;
    }

    private static List<int> GetPairAndIncrement(int[] nums, int i, ref int low, ref int high)
    {
        var pair = new List<int> { nums[i], nums[low], nums[high] };
        do
            low++;
        while (low < high && nums[low] == nums[low - 1]);

        do
            high--;
        while (low < high && nums[high] == nums[high + 1]);

        return pair;
    }


    //Too Slow!
    private IEnumerable<IList<int>> ThreeSumRecursive(int[] nums, int index, IEnumerable<int> pairs)
    {
        var cur = pairs.ToList();
        if (cur.Count == 3)
        {
            if (cur.Sum() == 0)
                yield return cur
                    .OrderBy(i => i)
                    .ToList();
            yield break;
        }

        if (index >= nums.Length)
            yield break;

        var skippedPairs = ThreeSumRecursive(nums, index + 1, cur);
        foreach (var p in skippedPairs)
            yield return p;

        cur.Add(nums[index]);
        var includedPairs = ThreeSumRecursive(nums, index + 1, cur);
        foreach (var p in includedPairs)
            yield return p;
    }
}