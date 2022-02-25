using System;
using System.Collections.Generic;

namespace SearchAlgorithms
{
    public class BreadthFirstSearch : AbstractSearchType
    {
        public BreadthFirstSearch() : base() { }


        public override void AddNode(Node node)
        {
            // add to the end of the list
            Frontier.Add(node);
        }

        public override bool ContainsState((int row, int col) state)
        {
            foreach (Node node in Frontier)
            {
                if (node.State == state)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Empty()
        {
            return Frontier.Count.Equals(0);
        }

        public override Node RemoveNode()
        {
            if (Empty())
            {
                throw new Exception("Empty Frontier");
            }else
            {
                Node lNode = Frontier[0];
                Frontier.RemoveAt(0);
                return lNode;
            }
        }
    }
}
