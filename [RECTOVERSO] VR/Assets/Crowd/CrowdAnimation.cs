using UnityEngine;

public class CrowdAnimation : MonoBehaviour
{
    public AnimatorOverrideController overrideControllerTemplate;
    public AnimationClip[] walkClips;
    public AnimationClip[] waveClips;

    private Animator animatorChild;

    public void Initialize()
    {
        animatorChild = GetComponentInChildren<Animator>();

        if (animatorChild == null || overrideControllerTemplate == null)
        {
            Debug.LogError("Animator ou overrideControllerTemplate manquant !");
            return;
        }

        AnimatorOverrideController overrideController = new AnimatorOverrideController(overrideControllerTemplate);

        // Choix al�atoire
        AnimationClip randomWalk = walkClips[Random.Range(0, walkClips.Length)];
        AnimationClip randomWave = waveClips[Random.Range(0, waveClips.Length)];

        // Remplacements
        overrideController["Walk"] = randomWalk;
        overrideController["Waving1"] = randomWave;

        animatorChild.runtimeAnimatorController = overrideController;

        Debug.Log($"Marche : {randomWalk.name} / Salue : {randomWave.name}");
    }
}
