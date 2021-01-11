using System;

namespace CodeWars.DataStructures.SparseTable
{
	public class MinSparseTable : SparseTable
	{
		public MinSparseTable(int[] arr)
			: base(arr)
		{ }

		protected override int Action(int l, int r) => Math.Min(l, r);
	}
}
