using System;
using System.Collections.Generic;

namespace SearchAlgorithms
{
    public class Node
    {
        public Node((int row, int col) state, Node parent, Action action)
        {
            State = state;
            Parent = parent;
            Action = action;
        }

        public (int row, int col) State { get; }
        public Node Parent { get; }
        public Action Action { get; }
    }
}
