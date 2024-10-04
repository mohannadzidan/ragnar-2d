using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent()]
public class Navigator : MonoBehaviour
{
    public float moveSpeed = 1f;
    public int stopDistance = 0;

    [HideInInspector]
    public bool active = true;

    public Vector3 destination
    {
        get
        {
            return _destination;
        }
        set
        {
            GoTo(value);
            _destination = value;
        }
    }


    public bool reached
    {
        get
        {
            return !hasPath;

        }

    }

    public Vector3 direction => _direction;
    public float speed => _speed;


    private Vector3 _destination;
    private Vector3 _direction;

    private List<Vector3>.Enumerator path;
    private bool reachedCheckpoint = true;
    private bool hasPath = false;

    private PathFinder pathFinder;

    private float _speed;

    void Start()
    {
        pathFinder = GameObject.Find("Walkable")?.GetComponent<PathFinder>(); // Find the player GameObject by name
        if (pathFinder == null)
        {
            throw new System.Exception("Cannot find Walkable tilemap!, make sure there is a game object named `Walkable` and has component PathFinder in the scene!");
        }

    }


    int RoughDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    public void GoTo(Vector3 position, float speedRatio = 0, int stopDistance = -1)
    {
        if (stopDistance < 0) stopDistance = this.stopDistance;
        if (speedRatio <= 0)
        {
            _speed = moveSpeed;
        }
        else
        {
            _speed = Mathf.Clamp(speedRatio, 0, 1) * moveSpeed;
        }

        var destinationCell = pathFinder.GetCellPosition(position);
        Vector3Int currentDestinationCell = pathFinder.GetCellPosition(_destination);
        if (destinationCell == currentDestinationCell)
        {
            return;
        }

        Vector3Int currentCell = pathFinder.GetCellPosition(transform.position);
        if (RoughDistance(currentCell, destinationCell) <= stopDistance)
        {
            reachedCheckpoint = true;
            hasPath = false;
            return;
        }

        var result = pathFinder.FindPath(transform.position, position, stopDistance);
        if (result != null)
        {
            path = result.GetEnumerator();
            var hasNext = path.MoveNext();
            reachedCheckpoint = !hasNext;
            hasPath = hasNext;
            if (hasNext) _direction = (path.Current - transform.position).normalized;
        }
        else
        {
            reachedCheckpoint = true;
            hasPath = false;
        }
        _destination = position;
    }
    void Update()
    {
        if (hasPath && !reachedCheckpoint)
        {
            if (!active)
            {
                return;
            }
            var delta = path.Current - transform.position;
            delta.z = 0;  // Ignore z-axis
            var distanceToDestination = delta.magnitude;
            if (distanceToDestination > 0)
            {
                var step = _speed * Time.deltaTime;
                var dirToDest = delta / distanceToDestination;

                if (distanceToDestination < step)
                {
                    transform.position = path.Current;
                    reachedCheckpoint = true;
                }
                else
                {
                    transform.position += new Vector3(dirToDest.x, dirToDest.y, 0) * step;
                }

            }
        }
        else if (hasPath && path.MoveNext())
        {
            _direction = (path.Current - transform.position).normalized;
            reachedCheckpoint = false;
        }
        else
        {
            hasPath = false;

        }
    }

    public bool IsWalkable(Vector3 point)
    {
        return pathFinder.IsWalkable(point);
    }
}
