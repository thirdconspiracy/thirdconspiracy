using System;

namespace CodeWars.DataStructures.SparseTable
{
	public abstract class SparseTable
	{
		protected int[,] lookup;
		protected int k;

		protected SparseTable(int[] arr)
		{
			var n = arr.Length;
			k = (int) Math.Log2(n);
			lookup = new int[n, k + 1];
			BuildSparseTable(arr, n);
		}

		private void BuildSparseTable(int[] arr, int n)
		{
			for (var i = 0; i < n; i++)
				lookup[i, 0] = arr[i];

			for (var j = 1; j <= k; j++)
			for (var i = 0; i + (1 << j) - 1 < n; i++)
			{
				var left = lookup[i, j - 1];
				var index = i + (1 << (j - 1));
				var right = lookup[index, j - 1];
				lookup[i, j] = Action(left, right);
			}
		}

		public virtual int Query(int l, int r)
		{
			var j = (int)Math.Log2(r - l + 1);
			var left = lookup[l, j];
			var index = r - (1 << j) + 1;
			var right = lookup[index, j];
			return Action(left, right);
		}

		protected abstract int Action(int l, int r);

	}
}
