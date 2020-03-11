using System;
using System.Collections.Generic;
using System.Text;

namespace thirdconspiracy.Math.CycleDetection
{
	public class FloydCycleDetection
	{
		/// <summary>
		/// Given
		/// * an array of integers
		/// Where
		/// * length is n + 1
		/// * and values are between 1 and n (inclusive)
		/// * and there is one and only one duplicate number
		/// Prove
		/// * that at least one duplicate number must exist
		/// * and determine that duplicate number
		///
		/// Solution Constraints
		/// 1. You must not modify the array (assume the array is read only).
		/// 2. You must use only constant, O(1) extra space.
		/// 3. Your runtime complexity should be less than O(n*n).
		/// 4. There is only one duplicate number in the array, but it could be repeated more than once.
		/// </summary>
		/// <param name="nums">array containing n + 1 integers where each integer is between 1 and n (inclusive)</param>
		/// <returns></returns>
		public int FindDuplicate(int[] nums)
		{
			var tortoise = nums[0];
			var hare = nums[0];

			//1: There are only 2 options
			// * All numbers are unique where, if sorted, the result would be 1-n
			// * One or more duplicates where, if sorted, you'd have 1-n but a few numbers skipped to allow for duplicates
			//2: If there is a duplicate (always will be, otherwise number will be greater than n), hare will meet the tortoise
			do
			{
				tortoise = nums[tortoise];
				hare = nums[nums[hare]];
			} while (tortoise != hare);

			//Given they met, determine duplicate
			hare = nums[0];
			while (tortoise != hare)
			{
				tortoise = nums[tortoise];
				hare = nums[hare];
			}

			return tortoise;
        }

		/// <summary>
		/// Given a linked list collection,
		/// determine if a cycle exists and where that cycle starts
		/// </summary>
		/// <param name="head"></param>
		/// <returns></returns>
		public LinkedListNode<int> DetectCycle(LinkedListNode<int> head)
		{
			if (head?.Next?.Next == null)
			{
				return null;
			}

			var tortoise = head;
			var hare = head;
			do
			{
				tortoise = tortoise.Next;
				hare = hare.Next.Next;
			} while (hare.Next != null && hare != tortoise);

			hare = head;
			while (hare != tortoise)
			{
				hare = hare.Next;
				tortoise = tortoise.Next;
			}

			return hare;
		}

	}
}
