using System;
using System.Collections.Generic;
using System.Linq;
using thirdconspiracy.Utilities.Extensions;

namespace thirdconspiracy.Math
{
    public static class Median
    {

        public static string[] RunningMedian(int[] numbers, string[] verify)
        {
            var sortedList = new List<int>();
            var runningMedian = new string[numbers.Length];
            for (int i = 0; i < numbers.Length; i++)
            {
	            sortedList.SortedInsert(numbers[i]);

                var instanceCount = i + 1;
                if (instanceCount == 1)
                {
                    var avg = sortedList[0];
                    runningMedian[i] = $"{avg:F1}";
                }
                else if (instanceCount % 2 == 0)
                {
                    var lower = i / 2;
                    var avg = (decimal)(sortedList[lower] + sortedList[lower + 1]) / 2;
                    runningMedian[i] = $"{avg:F1}";
                }
                else
                {
                    int lower = i / 2;
                    var avg = sortedList[lower];
                    runningMedian[i] = $"{avg:F1}";
                }
            }

            return runningMedian;
        }

        public static string[] RunningMedian_slow(int[] numbers, string[] verify)
        {
            var runningMedian = new string[numbers.Length];
            if (numbers.Length == 0)
            {
                return runningMedian;
            }

            runningMedian[0] = GetDecimal(numbers[0]);

            if (numbers.Length == 1)
                return runningMedian;

            var ll = new LinkedList<int>();
            var lr = new LinkedList<int>();

            ll.AddFirst(numbers[0]);
            if (numbers[1] < numbers[0])
            {
                Insert(ll, numbers[1]);
                MoveRight(ll, lr);
            }
            else
            {
                lr.AddFirst(numbers[1]);
            }
            runningMedian[1] = GetAverage(ll, lr);

            var isEven = true;
            var balance = 0;
            for (var i = 2; i < numbers.Length; i++)
            {
                var num = numbers[i];
                if (num < lr.First.Value)
                {
                    Insert(ll, num);
                    balance -= 1;
                }
                else
                {
                    Insert(lr, num);
                    balance += 1;
                }
                isEven = !isEven;

                if (balance < -1)
                {
                    MoveRight(ll, lr);
                    balance += 2;
                }
                else if (balance > 0)
                {
                    MoveLeft(ll, lr);
                    balance -= 2;
                }

                var v = verify[i];

                if (!isEven)
                    runningMedian[i] = GetDecimal(ll.Last.Value);
                else
                    runningMedian[i] = GetAverage(ll, lr);
            }

            return runningMedian;
        }

        private static void Insert(LinkedList<int> ll, int num)
        {
            if (ll == null)
            {
                ll.AddFirst(num);
                return;
            }

            var node = ll.First;
            do
            {
                if (num < node.Value)
                {
                    ll.AddBefore(node, num);
                    return;
                }

                node = node.Next;
            } while (node != null && node != ll.Last);

            if (node != null && num < node.Value)
                ll.AddBefore(node, num);
            else
                ll.AddLast(num);
        }

        private static string GetDecimal(int num)
        {
            return $"{num:F1}";
        }

        private static string GetAverage(LinkedList<int> ll, LinkedList<int> lr)
        {
            var avg = (decimal)(ll.Last.Value + lr.First.Value) / 2;
            return $"{avg:F1}";
        }

        private static void MoveRight(LinkedList<int> ll, LinkedList<int> lr)
        {
            var tmp = ll.Last.Value;
            ll.RemoveLast();
            lr.AddFirst(tmp);
        }

        private static void MoveLeft(LinkedList<int> ll, LinkedList<int> lr)
        {
            var tmp = lr.First.Value;
            lr.RemoveFirst();
            ll.AddLast(tmp);
        }

    }
}
