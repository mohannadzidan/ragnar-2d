using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // The prefab to spawn
    public GameObject prefab;

    // Spawn settings
    public float spawnRadius = 5f;
    public float timeBetweenSpawns = 2f; // Time between each spawn
    public int maxAliveEntities = 5;     // Maximum number of entities alive at the same time

    // Internal state
    private List<GameObject> aliveEntities = new List<GameObject>();  // List to track spawned entities
    private float spawnTimer = 0f;                                    // Timer to keep track of spawn intervals
    private PathFinder pathfinder;


    void Start()
    {
        pathfinder = GameObject.Find("Walkable")?.GetComponent<PathFinder>(); // Find the player GameObject by name
        if (pathfinder == null)
        {
            throw new System.Exception("Cannot find Walkable tilemap!, make sure there is a game object named `Walkable` and has component PathFinder in the scene!");
        }
        for (int i = 0; i < maxAliveEntities; i++)
        {
            SpawnEntity();
        }

    }

    void Update()
    {
        // If there are fewer than the max allowed entities, spawn new ones based on the timer
        if (aliveEntities.Count < maxAliveEntities)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= timeBetweenSpawns)
            {
                SpawnEntity();
                spawnTimer = 0f;  // Reset timer
            }
        }

        // Clean up the list by removing any destroyed entities
        aliveEntities.RemoveAll(entity => entity == null);
    }

    // Method to spawn a new entity
    void SpawnEntity()
    {
        var position = Random.insideUnitCircle * spawnRadius + (Vector2)(transform.position);
        for (int i = 0; i < 5; i++)
        {
            if (pathfinder.IsWalkable(position))
            {
                break;
            }
            else if (i == 4)
            {
                return;
            }
            else
            {
                position = Random.insideUnitCircle * spawnRadius + (Vector2)(transform.position);
            }
        }

        // Spawn the prefab at the spawner's position and rotation
        GameObject newEntity = Instantiate(prefab, position, transform.rotation);

        // Add the newly spawned entity to the list of alive entities
        aliveEntities.Add(newEntity);
    }

    // Debugging visualization in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, spawnRadius); // Draw a sphere to visualize the spawner
    }
}
