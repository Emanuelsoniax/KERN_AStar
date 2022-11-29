using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>

    private List<Node> openNodes = new List<Node>();       //keeps track of nodes open for search
    private List<Node> closedNodes = new List<Node>();     //keeps track of nodes closed for search
    private Node current;

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Vector2Int> pathToTarget = new List<Vector2Int>();
        openNodes.Clear();
        closedNodes.Clear();

        Node startNode = CreateNode(startPos, endPos, startPos, null);

        openNodes.Add(startNode);

        while (openNodes.Count != 0)
        {
            current = GetLowestFNode();
            openNodes.Remove(current);
            closedNodes.Add(current);

            if (current.position == endPos)
            {
                break;
            }

            Search(endPos, grid, current);
        }
        while (current.parent != null)
        {
            pathToTarget.Add(current.position);
            current = current.parent;
        }

        pathToTarget.Reverse();
        return pathToTarget;
    }

    private Node GetLowestFNode()
    {
        Node currentNode;
        Node last = null;
        Node final = null;

        foreach (Node n in openNodes)
        {
            currentNode = n;
            if (last == null || currentNode.FScore < last.FScore)
            {
                final = currentNode;
            }
            last = currentNode;
        }
        return final;
    }

    private void Search(Vector2Int endPos, Cell[,] grid, Node currentNode)
    {
        openNodes.Remove(currentNode); closedNodes.Add(currentNode);

        List<Node> neighbours = GetNeighbourNodes(currentNode.position, endPos, currentNode, grid);
        //List<Node> sortedList = neighbours.OrderBy(n => n.FScore).ToList();

        foreach (Node neighbour in neighbours)
        {
            if (closedNodes.Contains(neighbour))
            {
                continue;
            }

            if (ContainsWall(currentNode, neighbour, grid))
            {
                closedNodes.Add(neighbour);
                continue;
            }

            if (neighbour.FScore <= currentNode.FScore || !openNodes.Contains(neighbour))
            {
                neighbour.GScore = currentNode.GScore;
                neighbour.HScore += currentNode.HScore;
                neighbour.parent = currentNode;
                if (!openNodes.Contains(neighbour))
                {
                    openNodes.Add(neighbour);
                }
            }
        }
    }

    private bool ContainsWall(Node currentNode, Node neighbourToCheck, Cell[,] grid)
    {
        //check waar de muur zit relatief tot de speler
        //als er een muur is, verwijderen uit lijst
        Vector2Int neighbourDir = currentNode.position - neighbourToCheck.position;
        Cell neighbourCell = grid[neighbourToCheck.position.x, neighbourToCheck.position.y];

        if (neighbourDir.Equals(new Vector2Int(1, 0)))
        {
            if (neighbourCell.HasWall(Wall.RIGHT))
            {
                return true;
            }
        }
        else if (neighbourDir.Equals(new Vector2Int(-1, 0)))
        {
            if (neighbourCell.HasWall(Wall.LEFT))
            {
                return true;
            }
        }
        else if (neighbourDir.Equals(new Vector2Int(0, 1)))
        {
            if (neighbourCell.HasWall(Wall.UP))
            {
                return true;
            }
        }
        else if (neighbourDir.Equals(new Vector2Int(0, -1)))
        {
            if (neighbourCell.HasWall(Wall.DOWN))
            {
                return true;
            }
        }
        return false;
    }

    public List<Node> GetNeighbourNodes(Vector2Int startPos, Vector2Int endPos, Node parent, Cell[,] grid)
    {
        List<Cell> neighbourCells = new List<Cell>();

        foreach (Cell cell in grid)
        {
            if (cell.gridPosition == parent.position)
            {
                neighbourCells = cell.GetNeighbours(grid);
            }
        }

        List<Node> neighbourNodes = new List<Node>();

        foreach (Cell cell in neighbourCells)
        {
            Node node = CreateNode(startPos, endPos, cell.gridPosition, parent);
            neighbourNodes.Add(node);
        }

        return neighbourNodes;
    }

    private Node CreateNode(Vector2Int startPos, Vector2Int endPos, Vector2Int position, Node parent)
    {
        Node newNode = new Node();
        newNode.position = position;
        newNode.parent = parent;
        newNode.GScore = Vector2Int.Distance(startPos, position);
        newNode.HScore = Vector2Int.Distance(endPos, position);

        Debug.Log("Node created");
        return newNode;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
