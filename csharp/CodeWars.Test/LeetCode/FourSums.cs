using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class FourSums
{
    [TestCase(new[] { -2,-1,-1,1,1,2,2 }, 0, "[[-2,-1,1,2],[-1,-1,1,1]]")]
    [TestCase(new[] { 1,0,-1,0,-2,2 }, 0, "[[-2,-1,1,2],[-2,0,0,2],[-1,0,0,1]]")]
    [TestCase(new[] { 2,2,2,2,2 }, 8, "[[2,2,2,2]]")]
    public void TestCases(int[] nums, int target, string json)
    {
        var expected = JsonConvert.DeserializeObject<List<List<int>>>(json);
        var actual = FourSum(nums, target);
        CollectionAssert.AreEquivalent(expected, actual);
    }
    
    public IList<IList<int>> FourSum(int[] nums, int target)
    {
        if (nums.Length < 3)
            return new List<IList<int>>();
        
        Array.Sort(nums);

        var endPairs = new Dictionary<int, List<KeyValuePair<int, int>>>();
        for (var i = 2; i < nums.Length - 1; i++)
        {
            if (i < nums.Length - 2 && nums[i] == nums[i + 1])
                continue;
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (j < nums.Length - 1 && nums[j] == nums[j + 1])
                    continue;
                
                var total = nums[i] + nums[j];
                if (endPairs.ContainsKey(total))
                    endPairs[total].Add(new KeyValuePair<int, int>(i, j));
                else
                {
                    endPairs[total] = new List<KeyValuePair<int, int>>
                    { new(i, j) };
                }
            }
        }
        
        var results = new List<IList<int>>();
        for (var i = 0; i < nums.Length - 3; i++)
        {
            if (i > 0 && nums[i] == nums[i-1])
                continue;
            for (var j = i + 1; j < nums.Length - 2; j++)
            {
                if (j > i+1 && nums[j] == nums[j-1])
                    continue;
                
                var expected = target - nums[i] - nums[j];
                if (!endPairs.ContainsKey(expected))
                    continue;
                
                foreach (var kvp in endPairs[expected])
                {
                    if (j >= kvp.Key)
                        continue;
                    results.Add(new List<int> { nums[i], nums[j], nums[kvp.Key], nums[kvp.Value] });
                }
            }
        }

        return results;
    }
}