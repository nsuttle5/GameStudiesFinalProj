using UnityEngine;
using TMPro;

public class InteractableNote : MonoBehaviour
{
    public GameObject noteUI; // Reference to the UI GameObject
    public TMP_Text noteText; // Reference to the TMP_Text component
    public string noteContent; // The content of the note

    private bool isPlayerInRange = false;
    private bool isNoteDisplayed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        noteUI.SetActive(false); // Hide the UI at the start
        Debug.Log("Note UI is hidden at the start.");
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isNoteDisplayed)
            {
                noteUI.SetActive(false); // Hide the UI if it is already displayed
                isNoteDisplayed = false;
                Debug.Log("Player pressed E again. Note UI hidden.");
            }
            else
            {
                noteText.text = noteContent; // Set the text of the note
                noteUI.SetActive(true); // Show the UI when E is pressed
                isNoteDisplayed = true;
                Debug.Log("Player pressed E. Note content displayed.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered the trigger zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            noteUI.SetActive(false); // Hide the UI when the player leaves the trigger
            isNoteDisplayed = false;
            Debug.Log("Player exited the trigger zone. Note UI hidden.");
        }
    }
}