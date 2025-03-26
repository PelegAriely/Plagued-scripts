using UnityEngine;
using StarterAssets;

public class NoteInteraction : MonoBehaviour
{
    public Transform noteReadingPosition; // Where the note will be displayed
    private ThirdPersonController playerController;

    private GameObject currentNote;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    private bool isReading = false;  // To track if the player is reading a note
    
    void Start()
    {
        playerController = FindObjectOfType<ThirdPersonController>();
        if (playerController == null) Debug.LogError("ThirdPersonController not found in scene!");
    }

    public void HandleNoteInteraction(GameObject note)
    {
        if (isReading) // If the player is already reading the note
        {
            PutDownNote(); // Put down the note if reading
        }
        else if (note != null) // If the player is not reading a note and the note is valid
        {
            PickUpNote(note); // Pick up the note
        }
    }

    private void PickUpNote(GameObject note)
    {
        if (note == null) return; // Make sure the note exists

        currentNote = note;
        originalParent = note.transform.parent;
        originalPosition = note.transform.position;
        originalRotation = note.transform.rotation;

        if (noteReadingPosition != null)
        {
            note.transform.SetParent(noteReadingPosition);
            note.transform.localPosition = Vector3.zero;
            note.transform.localRotation = Quaternion.identity;
        }

        isReading = true;
        if (playerController != null) playerController.enabled = false; // Disable player movement while reading
    }

    public void PutDownNote()
    {
        if (currentNote == null || !isReading) return; // Only put down if we're holding a note

        currentNote.transform.SetParent(originalParent);
        currentNote.transform.position = originalPosition;
        currentNote.transform.rotation = originalRotation;

        isReading = false; // Set the state to not reading
        currentNote = null;

        if (playerController != null) playerController.enabled = true; // Re-enable player movement after putting down the note
    }
}
