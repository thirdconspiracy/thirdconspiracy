using System;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode;

public class ThreeSumClosestTests
{
    [TestCase(new[] { -1, 2, 1, -4 }, 1, 2)]
    [TestCase(new[] { 0,0,0 }, 1, 0)]
    [TestCase(new[] { 1,1,1 }, 0, 3)]
    public void TestCase(int[] nums, int target, int expected)
    {
        var actual = ThreeSumClosest(nums, target);
        Assert.AreEqual(expected, actual);
    }
    
    public int ThreeSumClosest(int[] nums, int target)
    {
        Array.Sort(nums);
        var distance = int.MaxValue;
        var actual = 0;
        for (var i = 0; i < nums.Length - 2; i++)
        {
            if (i > 0 && nums[i] == nums[i - 1])
                continue;
            
            var low = i + 1;
            var high = nums.Length - 1;
            while (low < high)
            {
                var sum = nums[i] + nums[low] + nums[high];
                if (sum == target)
                    return target;
                
                if (sum < target)
                {
                    var curDistance = Math.Abs(target - sum);
                    if (curDistance < distance)
                    {
                        distance = curDistance;
                        actual = sum;
                    }
                        
                    low++;
                }
                else if (sum > target)
                {
                    var curDistance = Math.Abs(target - sum);
                    if (curDistance < distance)
                    {
                        distance = curDistance;
                        actual = sum;
                    }
                    high--;
                }
            }
        }

        return actual;
    }
}