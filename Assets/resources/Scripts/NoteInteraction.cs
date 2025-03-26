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

    private bool isReading = false;

    void Start()
    {
        playerController = FindObjectOfType<ThirdPersonController>();
        if (playerController == null) Debug.LogError("ThirdPersonController not found in scene!");
    }

    public void HandleNoteInteraction(GameObject note)
    {
        if (isReading) return; // Prevents interacting with a new note while already reading one
        PickUpNote(note);
    }


    private void PickUpNote(GameObject note)
    {
        if (note == null) return;

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
        if (playerController != null) playerController.enabled = false;

    }

    public void PutDownNote()
    {
        if (currentNote == null || !isReading) return;

        currentNote.transform.SetParent(originalParent);
        currentNote.transform.position = originalPosition;
        currentNote.transform.rotation = originalRotation;

        isReading = false;
        currentNote = null;

        if (playerController != null) playerController.enabled = true;

    }
}

   
