using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
   // Singleton instance so other scripts can easily access this manager
   public static PuzzleManager Instance;

   private PuzzleObject[] puzzleObjects;
   
   private bool puzzleSolved = false;   // Flag to check if puzzle has already been solved
   
   [SerializeField] private Animator puzzleSolvedAnimator; // Animator that plays when the puzzle is completed

   private void Awake()
   {
      // Standard Singleton setup (only one PuzzleManager should exist)
      if (Instance == null) Instance = this;
         else Destroy(gameObject);
      
      puzzleObjects = FindObjectsOfType<PuzzleObject>();
   }
   
   // Check if all puzzle objects are placed correctly
   public void EvaluatePuzzleState()
   {
      if (puzzleSolved) return;

      foreach (PuzzleObject obj in puzzleObjects)
      {
         if (!obj.IsCorrectlyPlaced())
            return; // At least one object is incorrect
      }

      // All objects are in the correct place
      SolvePuzzle();
   }
   
   // Handles puzzle completion
   private void SolvePuzzle()
   {
      puzzleSolved = true;
      Debug.Log("Puzzle Solved!");

      // Play solved animation
      if (puzzleSolvedAnimator != null)
      {
         puzzleSolvedAnimator.SetTrigger("solve");
      }

      // Lock all puzzle objects
      foreach (var obj in puzzleObjects)
      {
         obj.LockInPlace();
      }
   }
   
   // Public getter to check if the puzzle is solved (used by other scripts)
   public bool IsPuzzleSolved() => puzzleSolved;
}
