﻿using NUnit.Framework;

namespace CodeWars.Test
{
    public class NodeWarsTests
    {
        //Number of nodes below root
        [Test]
        public void HeigthTest()
        {
            var nodeWars = new NodeWars();
            var root = SeedNode(nodeWars);

            var height = nodeWars.Height(root);
            Assert.AreEqual(4, height);
        }

        //Branch path between maximum number of nodes
        [Test]
        public void DistanceTest()
        {
            var nodeWars = new NodeWars();
            var root = SeedNode(nodeWars);

            var distance = nodeWars.Distance(root);
            Assert.AreEqual(7, distance);
        }

        [Test]
        public void TestLevels()
        {
            var expectedLevels = "3,1,5,2,4,8,6,9,7";
            var nodeWars = new NodeWars();
            var root = SeedNode(nodeWars);

            var levels = nodeWars.PrintLevels(root);
            var actualLevels = string.Join(",", levels);
            Assert.AreEqual(expectedLevels, actualLevels);
        }

        private static NodeWars.Node<int> SeedNode(NodeWars nodeWars)
        {
            NodeWars.Node<int> root = null;
            var numbers = new[] { 3, 5, 1, 8, 2, 4, 6, 9, 7 };
            foreach (var number in numbers)
                root = nodeWars.Insert(root, number);
            return root;
        }

    }
}
