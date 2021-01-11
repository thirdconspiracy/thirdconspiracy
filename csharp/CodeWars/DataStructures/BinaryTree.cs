using System;
using System.Collections.Generic;

namespace CodeWars.DataStructures
{
    public class BinaryTree
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

        public List<int> TraverseLevels(Node<int> root)
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

        /// <summary>
        /// All values in sorted order
        /// </summary>
        /// <param name="root"></param>
        /// <param name="values"></param>
        public void TraverseInOrder(Node<int> root, List<int> values)
        {
	        if (root == null)
		        return;
            TraverseInOrder(root.Left, values);
            values.Add(root.Data);
            TraverseInOrder(root.Right, values);
        }

        /// <summary>
        /// All values in reverse sorted order
        /// </summary>
        /// <param name="root"></param>
        /// <param name="values"></param>
        public void TraverseReverseOrder(Node<int> root, List<int> values)
        {
	        if (root == null)
		        return;
	        TraverseReverseOrder(root.Right, values);
	        values.Add(root.Data);
	        TraverseReverseOrder(root.Left, values);
        }

        /// <summary>
        /// Reverse of order values were added
        /// </summary>
        /// <param name="root"></param>
        /// <param name="values"></param>
        public void TraversePostOrder(Node<int> root, List<int> values)
        {
	        if (root == null)
		        return;
	        TraversePostOrder(root.Right, values);
	        TraversePostOrder(root.Left, values);
	        values.Add(root.Data);
        }

        /// <summary>
        /// Order values were added
        /// </summary>
        /// <param name="root"></param>
        /// <param name="values"></param>
        public void TraversePreOrder(Node<int> root, List<int> values)
        {
	        if (root == null)
		        return;
	        values.Add(root.Data);
            TraversePreOrder(root.Right, values);
	        TraversePreOrder(root.Left, values);
        }

    }
}
