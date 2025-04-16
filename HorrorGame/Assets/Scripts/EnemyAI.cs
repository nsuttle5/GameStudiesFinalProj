using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;             // Speed at which the enemy moves
    public float roamingRange = 10f;         // Maximum distance for roaming
    public float timeToChangeDirection = 2f; // Time before the enemy changes direction
    public float obstacleAvoidanceRange = 5f; // Range to detect obstacles
    public float raycastLength = 3f;         // Length of the raycast for detecting obstacles

    public float detectionRange = 15f;       // Range in which the enemy can spot the player
    public Transform player;                 // Reference to the player's position

    private Vector3 initialPosition;         // Starting position
    private Vector3 targetPosition;          // Random target position
    private float timeSinceLastChange = 0f;  // Timer to control direction change
    private bool isAvoidingObstacle = false; // Flag to track if we are avoiding an obstacle
    private bool isChasingPlayer = false;    // Flag to track if the enemy is chasing the player

    private NavMeshAgent navAgent;           // Reference to the NavMeshAgent

    public float maxSoundDetectionRadius = 20f; // Max distance for sound detection
    public float soundDetectionSensitivity = 0.5f; // How sensitive the enemy is to sound (player speed multiplier)

    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody for velocity

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on this GameObject");
        }

        // Store the initial position for boundary control
        initialPosition = transform.position;

        // Start roaming
        SetNewTargetPosition();

        // Get the player's Rigidbody for calculating speed
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Update the time counter for roaming logic
        timeSinceLastChange += Time.deltaTime;

        // Check for sound detection based on player's speed
        CheckForSoundDetection();

        // Check if the enemy can spot the player
        CheckForPlayer();

        // If the enemy is chasing the player, move towards the player using the NavMeshAgent
        if (isChasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            // If not chasing, check for obstacles and avoid them while roaming
            CheckForObstacles();

            if (isAvoidingObstacle)
            {
                AvoidObstacle();
            }
            else
            {
                // Move the enemy towards the target position
                navAgent.SetDestination(targetPosition);

                // If we reached the target position or enough time has passed
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f || timeSinceLastChange >= timeToChangeDirection)
                {
                    // Set a new target position after a delay or when close to the previous target
                    SetNewTargetPosition();
                }
            }
        }

        // Check for collision with the player
        CheckForPlayerCollision();
    }

    // Set a new target position within the roaming area
    private void SetNewTargetPosition()
    {
        // Reset the time counter
        timeSinceLastChange = 0f;

        // Generate a random position within a range of the initial position
        Vector3 randomDirection = new Vector3(
            Random.Range(-roamingRange, roamingRange),
            0f, // Assuming we are working with a 2D plane on the XZ-axis (you can change to Y if 2D)
            Random.Range(-roamingRange, roamingRange)
        );

        // Set the new target position
        targetPosition = initialPosition + randomDirection;
    }

    // Check if the enemy can spot the player (based on detection range)
    private void CheckForPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // If the player is within detection range and the enemy is not already chasing
            if (distanceToPlayer <= detectionRange)
            {
                // Check if the player is visible (not behind an obstacle)
                if (!IsPlayerBehindObstacle())
                {
                    isChasingPlayer = true;
                    navAgent.SetDestination(player.position); // Set the destination to the player's position
                }
                else
                {
                    // Player is behind an obstacle, stop chasing
                    isChasingPlayer = false;
                    SetNewTargetPosition(); // Go back to roaming
                }
            }
            else
            {
                // Player is out of detection range, stop chasing
                isChasingPlayer = false;
                SetNewTargetPosition(); // Go back to roaming
            }
        }
    }

    // Check if the player is behind an obstacle using raycasting
    private bool IsPlayerBehindObstacle()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - transform.position;

        // Cast a ray from the enemy to the player to check if there is an obstacle in between
        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Obstacle")) // Assuming obstacles have a tag named "Obstacle"
            {
                return true; // Player is behind an obstacle
            }
        }

        return false; // Player is not behind an obstacle
    }

    // Chase the player using the NavMeshAgent (handles pathfinding and obstacle avoidance)
    private void ChasePlayer()
    {
        if (navAgent != null)
        {
            // Set the destination to the player's position
            navAgent.SetDestination(player.position);
        }
    }

    // Check for obstacles in front of the enemy using Raycasting
    private void CheckForObstacles()
    {
        RaycastHit hit;

        // Cast a ray in front of the enemy (based on its current forward direction)
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceRange))
        {
            if (hit.collider != null)
            {
                // An obstacle is detected within range
                isAvoidingObstacle = true;
                return;
            }
        }

        // No obstacles detected, continue normal behavior
        isAvoidingObstacle = false;
    }

    // Avoid the obstacle by changing the movement direction
    private void AvoidObstacle()
    {
        // Use a random angle to steer the enemy around the obstacle
        float avoidAngle = Random.Range(90f, 180f); // Turn between 90 and 180 degrees

        // Rotate the enemy by a random angle around the Y-axis (for 3D)
        transform.Rotate(0f, avoidAngle, 0f);

        // Set a new target position after avoiding the obstacle
        SetNewTargetPosition();
    }

    // Detect collision with the player and end the game
    private void CheckForPlayerCollision()
    {
        if (Vector3.Distance(transform.position, player.position) < 1f) // Collision threshold (adjust as needed)
        {
            // End the game
            EndGame();
        }
    }

    // Game over logic
    private void EndGame()
    {
        // For simplicity, we reload the current scene (you could show a game over screen instead)
        Debug.Log("Game Over! Enemy caught the player!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reloads the current scene
    }

    // Debugging: visualize the raycast in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * raycastLength);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Show the detection range
    }

    // Function to handle sound detection based on player's speed
    private void CheckForSoundDetection()
    {
        if (playerRigidbody != null)
        {
            // Calculate the player's speed
            float playerSpeed = playerRigidbody.linearVelocity.magnitude; // This gives the speed of the player (in m/s)

            // Calculate the sound detection radius based on speed
            float soundRadius = Mathf.Clamp(playerSpeed * soundDetectionSensitivity, 0f, maxSoundDetectionRadius);

            // Check if the player is within the sound detection range
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= soundRadius)
            {
                // If within the sound detection radius and there are no obstacles
                if (!IsPlayerBehindObstacle())
                {
                    // Start chasing the player based on sound detection
                    isChasingPlayer = true;
                    navAgent.SetDestination(player.position);
                }
            }
        }
    }
}


