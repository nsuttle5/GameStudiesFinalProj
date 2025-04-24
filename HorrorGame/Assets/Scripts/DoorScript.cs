using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator doorAnimator; // Reference to the Animator component
    private bool isPlayerInTrigger = false; // Tracks if the player is in the trigger

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    void Update()
    {
        // Check if the player is in the trigger and presses the "E" key
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle the "IsOpen" parameter in the Animator
            bool isOpen = doorAnimator.GetBool("IsOpen");
            doorAnimator.SetBool("IsOpen", !isOpen);
        }
    }
}
