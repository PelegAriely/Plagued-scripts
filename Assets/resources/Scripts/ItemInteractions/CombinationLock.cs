using System;
using UnityEngine;
using StarterAssets;

public class CombinationLock : MonoBehaviour
{
    [Header("Dial Settings")]
    public Transform[] dials;
    public int[] correctCombination = { 3, 2, 7 };
    private int[] currentCombination = { 0, 0, 0 };
    private int selectedDial = 0;

    [Header("References")]
    public Animator boxAnimator;
    public string unlockTriggerName = "Open";
    public GameObject itemInside;
    public Transform lockModel;
    public Transform lockViewingPosition;
    public ThirdPersonController playerController;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    private bool isUnlocked = false;
    private bool isInteracting = false;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private Transform originalParent;

    private Action inputHandler = DoNothing;

    void Start()
    {
        if (itemInside) itemInside.SetActive(false);

        if (lockModel)
        {
            originalPos = lockModel.position;
            originalRot = lockModel.rotation;
            originalParent = lockModel.parent;
        }
    }

    void Update() => inputHandler();

    public bool IsUnlocked() => isUnlocked;

    public void StartInteraction()
    {
        if (isUnlocked || isInteracting || !lockModel || !lockViewingPosition) return;

        isInteracting = true;
        selectedDial = 0;

        // Move lock in front of camera
        lockModel.SetParent(lockViewingPosition);
        lockModel.localPosition = Vector3.zero;
        lockModel.localRotation = Quaternion.identity;

        LockPlayerMovement(true);

        inputHandler = HandleInput;
    }

    public void StopInteraction()
    {
        if (!isInteracting) return;

        isInteracting = false;

        lockModel.SetParent(originalParent);
        lockModel.position = originalPos;
        lockModel.rotation = originalRot;

        LockPlayerMovement(false);

        inputHandler = DoNothing;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            selectedDial = (selectedDial + 1) % 3;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            selectedDial = (selectedDial + 2) % 3;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            RotateDial(selectedDial, 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            RotateDial(selectedDial, -1);
        else if (Input.GetKeyDown(interactionKey))
            StopInteraction();
    }

    private void RotateDial(int dialIndex, int direction)
    {
        currentCombination[dialIndex] = (currentCombination[dialIndex] + direction + 10) % 10;
        dials[dialIndex].localRotation = Quaternion.Euler(0, 0, -currentCombination[dialIndex] * 36f);

        CheckCombination();
    }

    private void CheckCombination()
    {
        if (currentCombination[0] == correctCombination[0] &&
            currentCombination[1] == correctCombination[1] &&
            currentCombination[2] == correctCombination[2])
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        isUnlocked = true;

        if (boxAnimator)
            boxAnimator.SetTrigger(unlockTriggerName);

        if (itemInside)
            itemInside.SetActive(true);

        StopInteraction();
    }

    private void LockPlayerMovement(bool lockMovement)
    {
        if (playerController)
            playerController.enabled = !lockMovement;
    }

    private static void DoNothing() { }
}
