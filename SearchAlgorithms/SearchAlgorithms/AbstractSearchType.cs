using System;
using System.Collections.Generic;

namespace SearchAlgorithms
{
    public abstract class AbstractSearchType
    {
        private List<Node> frontier;
        public AbstractSearchType()
        {
            frontier = new List<Node>();
        }

        public List<Node> Frontier { get => frontier; set => frontier = value; }

        public abstract void AddNode(Node node);
        public abstract bool ContainsState((int row, int col) state);
        public abstract bool Empty();
        public abstract Node RemoveNode();

    }
}
