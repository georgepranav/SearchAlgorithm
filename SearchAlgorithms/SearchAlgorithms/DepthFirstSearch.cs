using System;
using System.Collections.Generic;

namespace SearchAlgorithms
{
    public class DepthFirstSearch : AbstractSearchType
    {
        public DepthFirstSearch() : base() { }


        public override void AddNode(Node node)
        {
           // add to the end of the list
            Frontier.Add(node);
        }

        public override bool ContainsState((int row, int col) state)
        {           
            foreach (Node node in Frontier)
            {
                if(node.State == state)
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
            }
            else
            {
                Node lNode = Frontier[^1]; // array[new Index(1, fromEnd: true)]
                Frontier.RemoveAt(Frontier.Count - 1);
                return lNode;
            }
        }
    }
}
