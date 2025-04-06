using UnityEngine;
using StarterAssets;
using TMPro;

public class LockedBox : MonoBehaviour
{
    public int correctCode = 327; // Set the correct code for the box 
    public GameObject objectInside;
    public Animator boxAnimator;
    public Transform lockModel;
    public Transform lockViewingPosition;
    public TMP_Text lockText;
    public TMP_Text enteredCodeText;
    public ThirdPersonController playerController;

    private string enteredCode = "";
    private bool isInteracting = false;
    private bool isUnlocked = false;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private Transform originalParent;



    void Start()
    {
        if (objectInside) objectInside.SetActive(false);
        if (lockModel)
        {
            originalPos = lockModel.position;
            originalRot = lockModel.rotation;
            originalParent = lockModel.parent;
        }

        lockText?.gameObject.SetActive(false);
        enteredCodeText?.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isInteracting) return;

        if (Input.anyKeyDown && enteredCode.Length < 3)
        {
            string key = Input.inputString;
            if (int.TryParse(key, out _))
            {
                enteredCode += key;
                enteredCodeText.text = enteredCode;

                if (enteredCode.Length == 3)
                {
                    if (int.Parse(enteredCode) == correctCode)
                    {
                        UnlockBox();
                    }
                    else
                    {
                        lockText.text = "Incorrect code. Try again.";
                        enteredCode = "";
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInteracting) StopInteraction();
            else StartInteraction();
        }
    }

    private void UnlockBox()
    {
        isUnlocked = true;
        lockText.text = "Unlocked!";
        boxAnimator.SetTrigger("Open");
        if (objectInside) objectInside.SetActive(true);
        StopInteraction();
    }

    public void StartInteraction()
    {
        if (isUnlocked || !lockModel || !lockViewingPosition) return;

        isInteracting = true;
        enteredCode = "";

        lockText?.gameObject.SetActive(true);
        lockText.text = "Enter 3-digit code";

        enteredCodeText?.gameObject.SetActive(true);
        enteredCodeText.text = "";

        lockModel.SetParent(lockViewingPosition);
        lockModel.localPosition = Vector3.zero;
        lockModel.localRotation = Quaternion.identity;

        LockPlayerMovement(true);
    }

    public void StopInteraction()
    {
        if (lockModel)
        {
            lockModel.SetParent(originalParent);
            lockModel.position = originalPos;
            lockModel.rotation = originalRot;
        }

        lockText?.gameObject.SetActive(false);
        enteredCodeText?.gameObject.SetActive(false);

        isInteracting = false;
        enteredCode = "";

        LockPlayerMovement(false);
    }


    private void LockPlayerMovement(bool lockMovement)
    {
        if (playerController)
            playerController.enabled = !lockMovement;
    }
}
