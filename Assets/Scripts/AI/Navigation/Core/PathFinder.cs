using System;
using System.Collections.Generic;
using Ragnar.Util;
using UnityEngine;
using UnityEngine.Tilemaps;



[DisallowMultipleComponent()]
public class PathFinder : Singleton<PathFinder>
{
    Tilemap walkable;

    void Awake()
    {
        current = this;
        walkable = GetComponent<Tilemap>(); // Find the player GameObject by name
        if (walkable == null)
        {
            throw new Exception("Cannot find Walkable tilemap!, make sure there is a game object named `Walkable` and has component Tilemap in the scene!");
        }
    }

    public Vector3Int GetCellPosition(Vector3 worldPosition)
    {
        return walkable.LocalToCell(walkable.WorldToLocal(GroundPosition(worldPosition)));
    }

    public List<Vector3> FindPath(Vector3 from, Vector3 targetWorldPos, int stopDistance = 0)
    {
        Vector3Int startCell = GetCellPosition(from);
        Vector3Int targetCell = GetCellPosition(targetWorldPos);
        if (!IsWalkable(targetCell)) return null;

        Node startNode = new Node(startCell);
        Node targetNode = new Node(targetCell);

        MinHeap<Node> openHeap = new MinHeap<Node>(1000); // MinHeap for the open list
        Dictionary<Vector3Int, Node> openDict = new Dictionary<Vector3Int, Node>(); // For fast lookups
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>(); // Closed list

        openHeap.Add(startNode);
        openDict[startCell] = startNode;

        var iterations = 2000;
        while (openHeap.Count > 0 && iterations-- > 0)
        {
            Node currentNode = openHeap.RemoveFirst();
            closedSet.Add(currentNode.position);

            // If target reached
            if (currentNode.position == targetNode.position)
            {
                return RetracePath(startNode, currentNode, stopDistance);
            }

            // Iterate through neighbors
            foreach (Vector3Int neighborPos in GetNeighbors(currentNode.position))
            {
                if (!IsWalkable(neighborPos) || closedSet.Contains(neighborPos))
                    continue;

                float newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode.position, neighborPos);
                Node neighbor;

                if (openDict.TryGetValue(neighborPos, out neighbor))
                {
                    if (newMovementCostToNeighbor < neighbor.gCost)
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor.position, targetNode.position);
                        neighbor.parent = currentNode;
                        openHeap.UpdateItem(neighbor);
                    }
                }
                else
                {
                    neighbor = new Node(neighborPos);
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor.position, targetNode.position);
                    neighbor.parent = currentNode;

                    openHeap.Add(neighbor);
                    openDict[neighborPos] = neighbor;
                }
            }
        }

        if (iterations <= 0) Debug.LogError("Infinite loop in pathfinding");
        return null; // No path found
    }

    public bool IsWalkable(Vector3 point)
    {
        Vector3Int targetCell = walkable.LocalToCell(walkable.WorldToLocal(GroundPosition(point)));
        return IsWalkable(targetCell);
    }

    List<Vector3> RetracePath(Node startNode, Node endNode, int stopDistance = 0)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;
        var prev = walkable.GetCellCenterWorld(currentNode.position);
        int steps = 0;
        while (currentNode != startNode)
        {
            Vector3 position = walkable.GetCellCenterWorld(currentNode.position);
            Debug.DrawLine(position, prev, Color.white, 3);
            prev = position;
            if (steps >= stopDistance) path.Add(position);
            currentNode = currentNode.parent;
            steps++;
        }
        path.Reverse();
        return path;
    }

    // Predefined directions for neighbors (4-way movement)
    public static readonly Vector3Int[] directions = {
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    IEnumerable<Vector3Int> GetNeighbors(Vector3Int currentPos)
    {
        foreach (var direction in directions)
        {
            yield return currentPos + direction;
        }
    }

    bool IsWalkable(Vector3Int position)
    {
        TileBase tile = walkable.GetTile(position);
        return tile != null;  // A tile is walkable if it's not null
    }

    float GetDistance(Vector3Int a, Vector3Int b)
    {
        int dstX = Mathf.Abs(a.x - b.x);
        int dstY = Mathf.Abs(a.y - b.y);
        return dstX + dstY;  // Using Manhattan distance for simplicity
    }

    Vector3 GroundPosition(Vector3 pos)
    {
        return new Vector3(pos.x, pos.y, 0);
    }
}
