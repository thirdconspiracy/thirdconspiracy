using System;
using System.Collections;
using System.Linq;
using CodeWars.DataStructures.SparseTable;
using NUnit.Framework;

namespace CodeWars.Test.LeetCode
{
    public class MedianOf2SortedArrays
    {
        [Test]
        public void Test1()
        {
            var arr1 = new[] { 4, 5, 6, 8, 9 };
            var arr2 = Array.Empty<int>();
            double expected = 6;

            var result = FindMedianSortedArrays(arr1, arr2);
            Assert.AreEqual(expected, result);
        }

        private double FindMedianSortedArrays(int[] nums1, int[] nums2)
        {
            //Short Circuit when possible
            if (nums1 == null || nums1.Length == 0)
            {
                if (nums2 == null || nums2.Length == 0)
                    return 0;
                else if (nums2.Length == 1)
                    return nums2[0];
            }
            else if (nums2 == null || nums2.Length == 0)
            {
                if (nums1.Length == 1)
                    return nums1[0];
            }

            //First should always be shortest
            // This is so that we're doing the binary search on the shortest array
            // so:
            //    O(long(min(n,m)))
            if (nums1.Length > nums2.Length)
                return FindMedianSortedArrays(nums2, nums1);

            var shouldAvg = (nums1.Length + nums2.Length) % 2 == 0;
            int middleRight = (nums1.Length + nums2.Length + 1) / 2;

            var x = nums1.Length;
            var y = nums2.Length;

            int low = 0;
            int high = x;
            while (low <= high)
            {
                int partitionX = (low + high) / 2;
                int partitionY = middleRight - partitionX;

                var xLeft = partitionX == 0
                    ? int.MinValue
                    : nums1[partitionX - 1];
                var xRight = partitionX == x
                    ? int.MaxValue
                    : nums1[partitionX];

                var yLeft = partitionY == 0
                    ? int.MinValue
                    : nums2[partitionY - 1];
                var yRight = partitionY == y
                    ? int.MaxValue
                    : nums2[partitionY];

                if (xLeft <= yRight && yLeft <= xRight)
                    return FindMedian(xLeft, xRight, yLeft, yRight, shouldAvg);
                else if (xLeft > yRight)
                    high = partitionX - 1;
                else
                    low = partitionX + 1;
            }

            throw new Exception("An error occurred");
        }

        private double FindMedian(int xLeft, int xRight, int yLeft, int yRight, bool shouldAvg)
        {
            double l = xLeft >= yLeft
                ? xLeft
                : yLeft;

            if (!shouldAvg)
                return l;

            double r = xRight <= yRight
                ? xRight
                : yRight;

            return (l + r) / 2;
        }
    }
}