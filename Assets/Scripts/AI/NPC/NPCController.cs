using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
[RequireComponent(typeof(IsoCharacterAnimator))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LivingEntity))]
[DisallowMultipleComponent()]
public class NPCController : MonoBehaviour
{
    public float moveRadius = 10f; // Configurable spawn radius for random movement
    public float moveInterval = 10f; // Configurable time interval for random movement
    public float chaseRadius = 15f; // Configurable radius to start chasing player
    public string playerName = "Player"; // Name of the player GameObject
    private LivingEntity player; // Reference to the player
    private Navigator navigator; // Reference to the custom PathFinder component
    private Vector3 spawnPosition; // Initial spawn position to calculate random movements

    private bool isChasing = false; // Flag to check if NPC is chasing the player

    private IsoCharacterAnimator isoAnim;
    private Animator anim;
    private LivingEntity entity;
    void Start()
    {
        navigator = GetComponent<Navigator>(); // Get the PathFinder component
        isoAnim = GetComponent<IsoCharacterAnimator>();
        anim = GetComponent<Animator>();
        entity = GetComponent<LivingEntity>();
        player = GameObject.Find(playerName)?.GetComponent<LivingEntity>();
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }


        if (navigator == null)
        {
            Debug.LogError("PathFinder component not found!");
            return;
        }

        spawnPosition = transform.position; // Save the NPC's initial position
        StartCoroutine(RandomMovement()); // Start random movement coroutine
        entity.OnDeath += () => { Destroy(this); };
    }

    void Update()
    {
        // Check distance between NPC and player
        if (!player.isDead && Vector3.Distance(transform.position, player.transform.position) <= chaseRadius)
        {
            // If within chase radius, chase the player
            ChasePlayer();
            if (navigator.reached)
            {
                isoAnim.LookAt(player.transform.position);
                isoAnim.Attack();
            }
            else
            {
                isoAnim.AbortAttack();
            }
        }
        else
        {
            // If not within chase radius, stop chasing
            isChasing = false;
        }
    }

    IEnumerator RandomMovement()
    {
        yield return new WaitForSeconds(moveInterval); // Wait for the specified interval
        while (true)
        {
            if (!isChasing && navigator.reached) // Only move randomly if not chasing the player
            {
                Vector3 randomPosition = GetRandomPositionWithinRadius();
                navigator.GoTo(randomPosition, 0.5f);
            }

            yield return new WaitForSeconds(moveInterval); // Wait for the specified interval
        }
    }

    void ChasePlayer()
    {
        if (!isChasing)
        {
            isChasing = true;
        }
        if (navigator.reached)
        {

        }
        navigator.GoTo(player.transform.position, 1);
    }

    // Generates a random position within the spawn radius
    Vector3 GetRandomPositionWithinRadius()
    {
        const int MAX_RETRIES = 10;
        for (int i = 0; i <= MAX_RETRIES; i++)
        {

            Vector2 randomPosition = Random.insideUnitCircle * moveRadius; // Random point inside a circle
            var pos = spawnPosition + new Vector3(randomPosition.x, randomPosition.y, 0); // Offset from the initial spawn position
            if (navigator.IsWalkable(pos)) return pos;
        }
        return spawnPosition;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, chaseRadius); // Draw a sphere to visualize the spawner
        Gizmos.color = new Color(1, 1, 1, 0.2f);
        Gizmos.DrawSphere(Application.isPlaying ? spawnPosition : transform.position, moveRadius); // Draw a sphere to visualize the spawner
    }

    void Attack()
    {
        if (Vector3.SqrMagnitude(player.transform.position - transform.position) > 1) return;
        player.Hit(Random.Range(3, 5));

    }
    void AnimationEventHandler(string eventName)
    {
        switch (eventName)
        {
            case "Attack":
                Attack();
                return;
            default: return;
        }
    }

}
