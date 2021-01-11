using System;

namespace CodeWars.DataStructures.SparseTable
{
	public class MaxSparseTable : SparseTable
	{

		public MaxSparseTable(int[] arr)
			: base(arr)
		{ }


		protected override int Action(int l, int r) => Math.Max(l, r);
	}
}
