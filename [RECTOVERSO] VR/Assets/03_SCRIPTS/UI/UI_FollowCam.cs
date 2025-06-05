using UnityEngine;

public class UI_FollowCam : MonoBehaviour
{
    public Camera mainCamera;
    public Transform targetObject;
    public Vector3 offset = new Vector3(0, 1, 2);
    public bool followCam;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned!");
        }
    }

    private void Update()
    {
        if (targetObject != null && mainCamera != null)
        {
            Vector3 targetPosition = targetObject.position + offset;
            if(followCam) {transform.LookAt(mainCamera.transform);}
            transform.Rotate(0, 180f, 0); 
            transform.position = targetPosition;
        }
    }
}
