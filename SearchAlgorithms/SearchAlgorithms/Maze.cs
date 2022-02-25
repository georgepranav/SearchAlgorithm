using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
namespace SearchAlgorithms
{
    public class Maze
    {
        private int fHeight;
        private int fWidth;

        private List<List<bool>> fWalls;

        private (int row, int col) fStart;
        private (int row, int col) fGoal;

        private int fNumExplored;
        private HashSet<(int row, int col)> fExplored;

        private (List<Action> actions, List<(int row, int col)> states) fSolutions;

        public int FNumExplored { get => fNumExplored; }

        public Maze()
        {
            fWalls = new List<List<bool>>();
            fStart = (0, 0);
            fGoal = (0, 0);
            fNumExplored = 0;
            fExplored = new HashSet<(int, int)>();
            fSolutions = (new List<Action>(), new List<(int, int)>());
        }

        public bool ReadFromFile(string FileName)
        {
            // Read file and set height and width of maze
            StreamReader file = null;

            try
            {
                file = new StreamReader(FileName);
                string line = file.ReadToEnd();

                Initialise(line);

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error: Couldn't locate the file " + FileName + "." + ex);
                return false;
            }
            finally
            {
                file.Close();
            }

            return true;

        }

        private void Initialise(string line)
        {
            Console.WriteLine(line.ToString());
            // Validate start and goal
            if (line.Count(character => character == 'A') != 1)
            {
                throw new Exception("maze must have exactly one start point");
            }
            if (line.Count(character => character == 'B') != 1)
            {
                throw new Exception("maze must have exactly one goal");
            }

            // Determine height and width of maze
            List<string> contents = line.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
            fHeight = contents.Count;

            fWidth = contents.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length;

            // Keep track of walls
            for (int i = 0; i < fHeight; i++)
            {

                List<bool> lRow = new List<bool>();
                for (int j = 0; j < fWidth; j++)
                {
                    try
                    {
                        if (contents[i][j] == 'A')
                        {
                            fStart = (i, j);
                            lRow.Add(false);
                        }
                        else if (contents[i][j] == 'B')
                        {
                            fGoal = (i, j);
                            lRow.Add(false);
                        }
                        else if (contents[i][j] == ' ')
                        {
                            lRow.Add(false);
                        }
                        else
                        {
                            lRow.Add(true);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        lRow.Add(false);
                    }
                }
                fWalls.Add(lRow);

            }
            fSolutions = (null, null);


        }

        public void Print()
        {
            List<(int, int)> lSolution;
            if (fSolutions != (null, null))
            {
                lSolution = fSolutions.states;
            }
            else
            {
                lSolution = null;
            }
            Console.WriteLine();
            foreach (var layers in fWalls.Select((row, i) => new { row, i }))
            {
                foreach (var brick in layers.row.Select((col, j) => new { col, j }))
                {
                    (int row, int col) lBrick = (layers.i, brick.j);


                    if (brick.col)
                    {
                        Console.Write("█");
                    }
                    else if (lBrick == fStart)
                    {
                        Console.Write("A");
                    }
                    else if (lBrick == fGoal)
                    {
                        Console.Write("B");
                    }

                    else if (lSolution != null && lSolution.Contains(lBrick))
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private List<(Action action, (int row, int col) state)> Neighbors((int row, int col) state)
        {
            int lRow = state.row;
            int lCol = state.col;

            List<(Action action, (int row, int col) state)> lCandidates = new List<(Action, (int, int))>();

            lCandidates.Add((Action.UP, (lRow - 1, lCol)));
            lCandidates.Add((Action.DOWN, (lRow + 1, lCol)));
            lCandidates.Add((Action.LEFT, (lRow, lCol - 1)));
            lCandidates.Add((Action.UP, (lRow, lCol + 1)));


            List<(Action action, (int row, int col) state)> lResult = new List<(Action, (int, int))>();

            foreach (var item in lCandidates)
            {
                Action action = item.action;
                int r = item.state.row;
                int c = item.state.col;
                if (0 <= r && r < fHeight && 0 <= c && c < fWidth)
                {
                    if (!fWalls[r][c])
                    {
                        lResult.Add((action, (r, c)));
                    }
                }
            }
            return lResult;
        }

        public void Solve(string method)
        {
            // find the solution to maze if exists

            // Initialise the frontier to just the starting position
            Node lStart = new Node(fStart, null, Action.NONE);
            AbstractSearchType lFrontier;
            if (method.Equals("BFS"))
            {
                lFrontier = new BreadthFirstSearch();

            }
            else if (method.Equals("DFS"))
            {
                lFrontier = new DepthFirstSearch();
            }
            else
            {
                throw new Exception("Method not found");
            }
            lFrontier.AddNode(lStart);

            //keep looping until solution found
            while (true)
            {
                //if nothing left in frontier than no solution
                if (lFrontier.Empty())
                {
                    throw new Exception("no solution");
                }

                // Choose a node from frontier
                Node lNode = lFrontier.RemoveNode();
                fNumExplored++;

                //if node is the goal, then we have a solution
                if (lNode.State == fGoal)
                {
                    List<Action> lActions = new List<Action>();
                    List<(int, int)> lCells = new List<(int, int)>();

                    while (lNode.Parent != null)
                    {

                        lActions.Add(lNode.Action);
                        lCells.Add(lNode.State);

                        lNode = lNode.Parent;
                    }
                    lActions.Reverse();
                    lCells.Reverse();

                    fSolutions = (lActions, lCells);
                    return;

                }
                //mark node as explored
                fExplored.Add(lNode.State);

                //add neighbors to frontier
                foreach (var item in Neighbors(lNode.State))
                {
                    var action = item.action;

                    (int row, int col) state = (item.state);

                    if (!lFrontier.ContainsState(state))
                    {
                        if (!fExplored.Contains(state))
                        {
                            Node lChild = new Node(state, lNode, action);
                            lFrontier.AddNode(lChild);
                        }
                    }
                }
            }

        }

        public void OutputImage(string fileName, bool showSolution = true, bool showExplored = false)
        {
            int lCellSize = 50;
            int lCellBorder = 2;

            Bitmap lImage = new Bitmap(fWidth * lCellSize, fHeight * lCellSize, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Graphics lGrafics = Graphics.FromImage(lImage);

            List<(int, int)> lSolution;
            if (fSolutions != (null, null))
            {
                lSolution = fSolutions.states;
            }
            else
            {
                lSolution = null;
            }
            Console.WriteLine();
            foreach (var layers in fWalls.Select((row, i) => new { row, i }))
            {
                foreach (var brick in layers.row.Select((col, j) => new { col, j }))
                {
                    (int i, int j) lBrick = (layers.i, brick.j);
                    Brush lFill;
                    // Walls
                    if (brick.col)
                    {
                        lFill =  new SolidBrush(Color.FromArgb(40, 40, 40)); 
                    }
                    //Start
                    else if (lBrick == fStart)
                    {
                        lFill = new SolidBrush(Color.FromArgb(255, 0, 0));
                    }
                    // Goal
                    else if (lBrick == fGoal)
                    {
                        lFill = new SolidBrush(Color.FromArgb(0, 171, 28));
                    }
                    // Solution
                    else if (lSolution != null && showSolution && lSolution.Contains(lBrick))
                    {
                        lFill = new SolidBrush(Color.FromArgb(220, 235, 113));
                    }
                    // Explored
                    else if (lSolution != null && showSolution && fExplored.Contains(lBrick))
                    {
                        lFill = new SolidBrush(Color.FromArgb(212, 97, 85));
                    }
                    // Empty Cell
                    else
                    {
                        lFill = new SolidBrush(Color.FromArgb(237, 240, 252));
                    }

                    lGrafics.FillRectangle(lFill,lBrick.j * lCellSize + lCellBorder, lBrick.i * lCellSize + lCellBorder, (lBrick.j + 1) * lCellSize - lCellBorder, (lBrick.i - 1) * lCellSize - lCellBorder);
                }
                lImage.Save(fileName);
            }
        }
    }
}
