using UnityEngine;

public class RespawnAndDeath : MonoBehaviour
{
    public Vector3 respawnPosition;
    public Health health;

    public Transform player;
    [SerializeField] private Rigidbody playerRB;
    public PlayerStateMachine playerStateMachine;
    public float spawnTime = 1.5f;

    public Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        respawnPosition = player.position; // Initialize respawn position to the player's starting position
    }

    // Update is called once per frame
    void Update()
    {
        DetectDeadCondition();
    }

    private void DetectDeadCondition()
    {
        if(health.currentHealth <= 0)
        {
            health.dead = true;
            playerStateMachine.OverrideState(PlayerStateMachine.PlayerStates.dead);

            Invoke(nameof(Respawn), spawnTime); // Respawn after 2 seconds
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        #region Enter hazard area and die
        if(other.CompareTag("Hazard"))
        {
            if (!health.dead)
            {
                health.currentHealth = 0;
                health.dead = true;
                playerStateMachine.OverrideState(PlayerStateMachine.PlayerStates.dead);

                Invoke(nameof(Respawn), spawnTime); // Respawn after 2 seconds
            }
        }
        #endregion

        #region Enter checkpoint and record the position
        if (other.CompareTag("Checkpoint"))
        {
            respawnPosition = other.transform.position;
            print("Checkpoint reached: " + respawnPosition);
        }

        #endregion
    }

    public void Respawn()
    {
        //playerStateMachine.OverrideState(PlayerStateMachine.PlayerStates.dead);
        health.dead = false;
        health.currentHealth = 3;
        player.position = respawnPosition + offset;
        playerRB.position = respawnPosition + offset;
        playerStateMachine.OverrideState(PlayerStateMachine.PlayerStates.airborne);
        print("Player respawned at: " + respawnPosition);
    }
}
