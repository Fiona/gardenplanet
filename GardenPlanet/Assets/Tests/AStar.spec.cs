using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace StompyBlondie
{
    [TestFixture]
    public class AStarTest
    {
        string[,] simpleTestMap =
        {
            {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "0", "0", "0", "0", "0", "1", "0"},
            {"0", "1", "1", "0", "1", "1", "1", "0", "1", "0"},
            {"0", "1", "1", "0", "1", "1", "1", "0", "1", "0"},
            {"0", "1", "0", "0", "0", "0", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "0", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
        };

        string[,] emptyTestMap =
        {
            {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
        };

        string[,] costTestMap =
        {
            {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
            {"0", "1", "1", "1", "4", "4", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "4", "4", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "4", "4", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "4", "4", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "4", "4", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "4", "4", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "4", "4", "1", "1", "1", "0"},
            {"0", "1", "1", "1", "1", "1", "1", "1", "1", "0"},
            {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0"}
        };


        class TestAStarImplementation : AStar
        {
            private string[,] tilemap;

            public TestAStarImplementation(string[,] map)
            {
                tilemap = map;
                /**
                Debug.Log("map -- ");
                for(var y = 0; y < tilemap.GetLength(0); y++)
                {
                    var line = "";
                    for(var x = 0; x < tilemap.GetLength(1); x++)
                    {
                        line += tilemap[y, x];
                    }
                    Debug.Log(line);
                }
                /**/
            }

            public override (float cost, float pathCost)? CostNode(AStarNode node)
            {
                var nodePos = ((string) node.value).Split(',');
                var x = int.Parse(nodePos[0]);
                var y = int.Parse(nodePos[1]);

                var endPos = ((string) end.value).Split(',');
                var endX = int.Parse(endPos[0]);
                var endY = int.Parse(endPos[1]);

                // Out of bounds
                if(x < 0 || y < 0 || y > tilemap.GetLength(0) - 1 || x > tilemap.GetLength(1) - 1)
                    return null;

                var value = int.Parse(tilemap[y, x]);

                // Impassable
                if(value == 0)
                    return null;

                // Work out path cost and send back
                var pathCost = node.previous?.pathCost + value ?? 0f;
                var cost = pathCost + Mathf.Sqrt(Mathf.Pow(endX - x, 2) + Mathf.Pow(endY - y, 2));
                return (cost, pathCost);
            }

            public override List<AStarNode> Expand(AStarNode node)
            {
                var nodePos = ((string) node.value).Split(',');
                var x = int.Parse(nodePos[0]);
                var y = int.Parse(nodePos[1]);
                var expansion = new List<AStarNode>();

                var posToCheck = new List<(int x, int y)> {(x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)};
                foreach(var pos in posToCheck)
                {
                    var newNode = new AStarNode {value = $"{pos.x},{pos.y}", previous = node};
                    // Cost pos
                    var cost = CostNode(newNode);
                    // Impassable
                    if(cost == null)
                        continue;
                    // Assign and go on
                    newNode.cost = cost.Value.cost;
                    newNode.pathCost = cost.Value.pathCost;
                    expansion.Add(newNode);
                }

                return expansion;
            }

            public override bool IsEndNode(AStarNode node)
            {
                return (string) node.value == (string) end.value;
            }
        }

        public List<(int x, int y)> GetPathList(List<AStarNode> path)
        {
            var expand = new List<(int x, int y)>();
            foreach(var n in path)
            {
                var nodePos = ((string) n.value).Split(',');
                var x = int.Parse(nodePos[0]);
                var y = int.Parse(nodePos[1]);
                expand.Add((x, y));
            }

            //foreach(var n in expand) Debug.Log(n);

            return expand;
        }

        [Test]
        public void TestSimpleHorizontalPath()
        {
            var astar = new TestAStarImplementation(emptyTestMap);
            var path = GetPathList(astar.Search("3,3", "6,3"));
            var expectedPath = new List<(int x, int y)>{
                (3, 3),
                (4, 3),
                (5, 3),
                (6, 3)
            };
            Assert.That(path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void TestSimpleVerticalPath()
        {
            var astar = new TestAStarImplementation(emptyTestMap);
            var path = GetPathList(astar.Search("3,2", "3,6"));
            var expectedPath = new List<(int x, int y)>{
                (3, 2),
                (3, 3),
                (3, 4),
                (3, 5),
                (3, 6)
            };
            Assert.That(path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void TestSimpleDiagnolPath()
        {
            var astar = new TestAStarImplementation(emptyTestMap);
            var path = GetPathList(astar.Search("3,2", "6,6"));
            var expectedPath = new List<(int x, int y)>{
                (3, 2),
                (3, 3),
                (4, 3),
                (4, 4),
                (5, 4),
                (5, 5),
                (6, 5),
                (6, 6)
            };
            Assert.That(path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void TestImpossibleStartNode()
        {
            var astar = new TestAStarImplementation(emptyTestMap);
            Assert.That(astar.Search("0,2", "6,6"), Is.Null);
        }

        [Test]
        public void TestImpossibleEndNode()
        {
            var astar = new TestAStarImplementation(emptyTestMap);
            Assert.That(astar.Search("3,3", "6,0"), Is.Null);
        }

        [Test]
        public void TestTraverseWall()
        {
            var astar = new TestAStarImplementation(simpleTestMap);
            var path = GetPathList(astar.Search("4,5", "4,7"));
            var expectedPath = new List<(int x, int y)>{
                (4, 5),
                (5, 5),
                (6, 5),
                (6, 6),
                (6, 7),
                (5, 7),
                (4, 7)
            };
            Assert.That(path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void TestConcavePath()
        {
            var astar = new TestAStarImplementation(simpleTestMap);
            var path = GetPathList(astar.Search("5,4", "2,2"));
            var expectedPath = new List<(int x, int y)>{
                (5, 4),
                (5, 5),
                (6, 5),
                (6, 6),
                (7, 6),
                (8, 6),
                (8, 5),
                (8, 4),
                (8, 3),
                (8, 2),
                (7, 2),
                (6, 2),
                (5, 2),
                (4, 2),
                (3, 2),
                (2, 2)
            };
            Assert.That(path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void TestPrefersEasierPath()
        {
            var astar = new TestAStarImplementation(costTestMap);
            var path = GetPathList(astar.Search("3,6", "6,8"));
            var expectedPath = new List<(int x, int y)>{
                (3, 6),
                (3, 7),
                (3, 8),
                (4, 8),
                (5, 8),
                (6, 8)
            };
            Assert.That(path, Is.EqualTo(expectedPath));
        }

        [Test]
        public void TestStillUsesSlightlyShittierPath()
        {
            var astar = new TestAStarImplementation(costTestMap);
            var path = GetPathList(astar.Search("3,1", "6,1"));
            var expectedPath = new List<(int x, int y)>{
                (3, 1),
                (4, 1),
                (5, 1),
                (6, 1)
            };
            Assert.That(path, Is.EqualTo(expectedPath));
        }

    }

}