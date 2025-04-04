using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private Animator boxAnimator; // Assign in Inspector
    [SerializeField] private GameObject itemInside; // Assign in Inspector
    private bool isOpen = false; // Prevents reopening

    public void OpenBox()
    {
        if (isOpen) return; // Prevent reopening

        isOpen = true;
        if (boxAnimator != null)
            boxAnimator.SetTrigger("open"); // ✅ Play animation

        if (itemInside != null)
        {
            itemInside.SetActive(true); // ✅ Show the item
            Debug.Log("Item inside the box is now visible: " + itemInside.name);
        }

        Debug.Log("Box opened: " + gameObject.name);
    }
}
