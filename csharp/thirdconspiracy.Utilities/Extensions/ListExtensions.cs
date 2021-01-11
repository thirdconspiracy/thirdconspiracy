using System;
using System.Collections.Generic;
using System.Text;

namespace thirdconspiracy.Utilities.Extensions
{
	public static class ListExtensions
	{
		public static void SortedInsert(this List<int> sortedList, int value)
		{
			var searchIndex = sortedList.BinarySearch(value);
			var insertIndex = searchIndex < 0
				? ~searchIndex //Bitwise complement of first greater element
				: searchIndex; //Found exact match
			sortedList.Insert(insertIndex, value);
		}
	}
}
