using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CodeWars
{
    public class NodeWars
    {
        public class Node<T>
        {
            public T Data { get; set; }
            public Node<T> Left { get; set; }
            public Node<T> Right { get; set; }
        }

        public Node<int> Insert(Node<int> root, int data)
        {
            if (root == null)
                root = new Node<int> { Data = data };
            else if (data <= root.Data)
                root.Left = Insert(root.Left, data);
            else
                root.Right = Insert(root.Right, data);

            return root;
        }

        public int Height(Node<int> root)
        {
            if (root == null)
                return 0;

            var left = root.Left == null ? 0 : Height(root.Left) + 1;
            var right = root.Right == null ? 0 : Height(root.Right) + 1;

            return Math.Max(left, right);
        }

        public int Distance(Node<int> root)
        {
            if (root == null)
                return 0;

            var leftHeight = Height(root.Left) + 1;
            var rightHeight = Height(root.Right) + 1;

            var centerDistance = leftHeight + rightHeight + 1;
            var leftDistance = Distance(root.Left);
            var rightDistance = Distance(root.Right);

            return Math.Max(centerDistance, Math.Max(leftDistance, rightDistance));
        }

        public List<int> PrintLevels(Node<int> root)
        {
            if (root == null)
                return new List<int>();

            var queue = new Queue<Node<int>>();
            queue.Enqueue(root);

            var result = new List<int>();
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();

                result.Add(cur.Data);

                if (cur.Left != null)
                    queue.Enqueue(cur.Left);
                if (cur.Right != null)
                    queue.Enqueue(cur.Right);
            }

            return result;
        }

    }
}
