using UnityEngine;

public class ObjectPickupTrigger : MonoBehaviour
{
    public GameObject fallingObject; // The object that will play the animation
    public Animator fallingAnimator; // Animator component
    public string animationTriggerName = "Fall"; // The trigger name for the animation
    public AudioSource soundEffect; // The sound effect to play

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // check if the player collides
        {
            // play the animation
            if (fallingAnimator != null)
            {
                fallingAnimator.SetTrigger(animationTriggerName);
            }

            // play the sound effect
            if (soundEffect != null)
            {
                soundEffect.Play();
            }
        }
    }
}
