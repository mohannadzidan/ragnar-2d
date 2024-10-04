using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Vector3Int position;
    public float gCost;  // Distance from start node
    public float hCost;  // Heuristic distance from the end node
    public Node parent;  // Parent node for retracing the path

    public Node(Vector3Int pos)
    {
        position = pos;
    }

    public float fCost => gCost + hCost;  // Total cost (gCost + hCost)
    private int heapIndex;

    // To implement priority queue via heap
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
