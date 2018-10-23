using System;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

namespace StompyBlondie
{
    public class AStarNode
    {
        public System.Object value;
        public AStarNode previous;
        public float cost;
        public float pathCost;
    }

    public class AStar
    {
        private AStarNode start;
        private AStarNode end;

        public virtual List<AStarNode> Search(System.Object start, System.Object end)
        {
            start = new AStarNode {value = start};
            end = new AStarNode {value = end};

            var list = new List<AStarNode>();
            return list;
        }

        public virtual (float cost, float costPath) CostNode()
        {
            return (cost: 0f, costPath: 0f);
        }

        private void StartSearch()
        {

        }

    }
}