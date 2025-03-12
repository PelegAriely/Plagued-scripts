using UnityEngine;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour
{
    private Camera mainCamera;
    private Camera activeCamera;
    private List<Camera> cameraStack = new List<Camera>(); // Keeps track of active cameras

    void Start()
    {
        mainCamera = Camera.main;
        activeCamera = mainCamera;
        mainCamera.enabled = true;

        // Disable all other cameras at the start
        Camera[] allCameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Camera cam in allCameras)
        {
            if (cam != mainCamera)
            {
                cam.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraTrigger"))
        {
            Camera newCamera = other.GetComponentInChildren<Camera>(); // Find camera in children
            if (newCamera != null && !cameraStack.Contains(newCamera))
            {
                cameraStack.Add(newCamera);
                SwitchCamera(newCamera);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraTrigger"))
        {
            Camera exitingCamera = other.GetComponentInChildren<Camera>();
            if (exitingCamera != null && cameraStack.Contains(exitingCamera))
            {
                cameraStack.Remove(exitingCamera);

                // Switch to the last active camera, or main camera if none left
                Camera nextCamera = cameraStack.Count > 0 ? cameraStack[cameraStack.Count - 1] : mainCamera;
                SwitchCamera(nextCamera);
            }
        }
    }

    private void SwitchCamera(Camera newCamera)
    {
        if (activeCamera != null)
        {
            activeCamera.enabled = false;
        }

        newCamera.enabled = true;
        activeCamera = newCamera;
    }
}
