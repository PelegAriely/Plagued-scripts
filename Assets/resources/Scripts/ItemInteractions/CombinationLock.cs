using System;
using System.Collections.Generic;
using UnityEngine;

public class CombinationLock : MonoBehaviour
{
    public Transform[] dials; // Array of the 3 rotating dials
    public int[] correctCombination = { 3, 2, 7 };
    private int[] currentCombination = { 0, 0, 0 };
    private int selectedDial = 0;

    [Header("Unlock Settings")]
    public Animator boxAnimator;
    public string unlockTriggerName = "Open";
    public GameObject itemInside;

    private bool isUnlocked = false;
    private bool isInteracting = false;
    public bool IsUnlocked => isUnlocked;

    private Dictionary<KeyCode, Action> inputActions;

    void Start()
    {
        if (itemInside != null)
            itemInside.SetActive(false);

        inputActions = new Dictionary<KeyCode, Action>
        {
            { KeyCode.RightArrow, () => selectedDial = (selectedDial + 1) % 3 },
            { KeyCode.LeftArrow, () => selectedDial = (selectedDial + 2) % 3 },
            { KeyCode.UpArrow, () => RotateDial(selectedDial, 1) },
            { KeyCode.DownArrow, () => RotateDial(selectedDial, -1) },
            { KeyCode.E, TryStopInteraction } // לצאת מהאינטרקציה
        };
    }

    void Update()
    {
        if (!isInteracting || isUnlocked) return;

        foreach (var entry in inputActions)
        {
            if (Input.GetKeyDown(entry.Key))
            {
                entry.Value.Invoke();
                break;
            }
        }
    }

    void RotateDial(int dialIndex, int direction)
    {
        currentCombination[dialIndex] = (currentCombination[dialIndex] + direction + 10) % 10;
        dials[dialIndex].localRotation = Quaternion.Euler(0, 0, -currentCombination[dialIndex] * 36f);
        CheckCombination();
    }

    void CheckCombination()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentCombination[i] != correctCombination[i])
                return;
        }

        Unlock();
    }

    void Unlock()
    {
        isUnlocked = true;
        isInteracting = false;

        Debug.Log("Lock opened!");

        if (boxAnimator != null)
            boxAnimator.SetTrigger(unlockTriggerName);

        if (itemInside != null)
            itemInside.SetActive(true);
    }

    public void StartInteraction()
    {
        if (isUnlocked) return;

        isInteracting = true;
    }

    private void TryStopInteraction()
    {
        isInteracting = false;
    }
    
    public void StopInteraction()
    {
        isInteracting = false;
    }
}
