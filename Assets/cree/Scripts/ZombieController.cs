using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField]
    private float detectionRange = 15f; // Detection range in units
    [SerializeField]
    private float fieldOfView = 90f; // Field of view in degrees
    [SerializeField]
    private float moveSpeed = 3f; // Movement speed
    [SerializeField]
    private float rotationSpeed = 2f; // Speed at which the enemy rotates to face the player

    private Transform player; // Player's transform
    private bool playerDetected = false; // Whether the player is detected
    private bool isChasing = false; // Whether the enemy is actively chasing the player

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        DetectPlayer();

        if (playerDetected)
        {
            RotateTowardsPlayer();
            print(IsFacingPlayer());
            if (IsFacingPlayer())
            {
                print("Avance");
                MoveTowardsPlayer();
            }
        }
    }

    // Check if the player is within the enemy's field of view and detection range
    private void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer < fieldOfView / 2f)
            {
                // Optionally, perform a raycast to ensure there's no obstacle between the enemy and the player
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        playerDetected = true;
                        return;
                    }
                }
            }
        }

        playerDetected = false; // Reset detection if player is out of range or view
    }

    // Smoothly rotate the enemy to face the player
    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Check if the enemy is facing the player within a small angle tolerance
    private bool IsFacingPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleToPlayer < 30f; 
    }

    // Move the enemy towards the player's position when facing them
    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Prevent the enemy from moving up or down
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    // Visualize the detection range and field of view in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw field of view lines
        Gizmos.color = Color.blue;
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, transform.up) * transform.forward * detectionRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, transform.up) * transform.forward * detectionRange;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }
}
