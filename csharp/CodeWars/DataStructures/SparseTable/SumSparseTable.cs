namespace CodeWars.DataStructures.SparseTable
{
	public class SumSparseTable : SparseTable
	{
		public SumSparseTable(int[] arr)
			: base(arr)
		{ }

		protected override int Action(int l, int r) => l + r;

		public override int Query(int l, int r)
		{
			var sum = 0;
			for (var j = k; j >= 0; j--)
			{
				if (1 << j <= r - l + 1)
				{
					sum += lookup[l, j];
					l += 1 << j;
				}
			}

			return sum;
		}
	}
}
