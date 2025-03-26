using UnityEngine;

public class HidingSpotScript : MonoBehaviour
{
    public Transform hidePosition; // Position where the player will hide
    public Transform exitPosition; // Position where the player will exit
    private bool isPlayerInZone = false; // Tracks if the player is in the trigger zone
    private GameObject player; // Reference to the player GameObject

    public PlayerScript playerScript;

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E))
        {
            if (!playerScript.isHiding)
            {
                HidePlayer();
            }
            else
            {
                ExitHidingSpot();
            }
        }
    }

    private void HidePlayer()
    {
        if (player != null)
        {
            player.transform.position = hidePosition.position; // Move player to hide position
            player.GetComponent<CharacterController>().enabled = false; // Disable movement
            playerScript.isHiding = true;
        }
    }

    private void ExitHidingSpot()
    {
        if (player != null)
        {
            player.transform.position = exitPosition.position; // Move player to exit position
            player.GetComponent<CharacterController>().enabled = true; // Enable movement
            playerScript.isHiding = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            player = other.gameObject; // Cache the player reference
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            player = null; // Clear the player reference
        }
    }
}
