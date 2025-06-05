using UnityEngine;

public class EyeAnimatorPerso2 : MonoBehaviour
{
    [Header("Eye Bones (Rig)")]
    public Transform eyeLeftBone;  // Bone de l'œil gauche
    public Transform eyeRightBone; // Bone de l'œil droit

    [Header("Camera to Look At")]
    public Transform targetCamera;

    [Header("Animation Settings")]
    public float rotationSpeed = 5f;

    [Header("Eye Rotation Limits")]
    public float maxYRotation = 75f; // Limite maximale sur l'axe Y
    public float maxZRotation = 75f; // Limite maximale sur l'axe Z

    // Offsets pour l'orientation des yeux (privés avec valeurs par défaut)
    private Vector3 leftEyeOffset = new Vector3(0, 75, 85);  // Offset pour l'œil gauche
    private Vector3 rightEyeOffset = new Vector3(0, 85, 75); // Offset pour l'œil droit


    void Update()
    {
        // Vérifier si la barre d'espace est enfoncée
        //if (Input.GetKey(KeyCode.Space))
        //{
            // Faire regarder les yeux vers la caméra avec des limites
            RotateEyeTowardsTarget(eyeLeftBone, targetCamera, leftEyeOffset);
            RotateEyeTowardsTarget(eyeRightBone, targetCamera, rightEyeOffset);
        //}
    }

    void RotateEyeTowardsTarget(Transform eyeBone, Transform target, Vector3 offset)
    {
        // Calculer la direction vers la cible
        Vector3 directionToTarget = target.position - eyeBone.position;

        // Appliquer LookAt
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized);

        // Ajouter un offset de rotation
        Quaternion rotationOffset = Quaternion.Euler(offset);
        targetRotation *= rotationOffset;

        // Convertir la rotation en angles d'Euler pour appliquer les limites
        Vector3 eulerRotation = targetRotation.eulerAngles;

        // Appliquer les limites sur les axes Y et Z
        eulerRotation.y = ClampAngle(eulerRotation.y, -maxYRotation, maxYRotation);
        eulerRotation.z = ClampAngle(eulerRotation.z, -maxZRotation, maxZRotation);

        // Garder l'axe X fixe (par exemple, à 0°)
        eulerRotation.x = 0f;

        // Appliquer la rotation limitée au bone
        eyeBone.localRotation = Quaternion.Euler(eulerRotation);
    }

    float ClampAngle(float angle, float min, float max)
    {
        // Assurer que l'angle est dans la plage [0, 360]
        if (angle > 180f) angle -= 360f;

        // Limiter l'angle
        return Mathf.Clamp(angle, min, max);
    }
}