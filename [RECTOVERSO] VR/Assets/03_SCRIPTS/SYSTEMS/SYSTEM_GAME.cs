using Oculus.Platform;
using Oculus.Platform.Models;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class SYSTEM_GAME : MonoBehaviour
{
    [Header("PREFABS")]
    public GameObject characterPrefab;
    public GameObject crowdPrefab;
    [Header("SCRIPTS")]
    public UI_FollowCam arrowFollowCamScript;
    public UI_FollowCam textFollowCamScript;
    public TMPTextTyper textTyperScript;
    public RandomSpriteChanger boxSpriteScript;
    public RandomSpriteChanger arrowSpriteScript;
    public Pencil02 pencilScript;
    public SYSTEM_DRAWZONE drawZoneScript;
    [Header("REFERENCES")]
    public Transform repairSpawn;
    public Transform center;
    public GameObject currentDamaged;
    public GameObject passthrough;
    public GameObject Pencil;
    public GameObject penBase;
    public GameObject playerObject;
    public AudioSource musicAudioSource;
    private SYSTEM_CHARACTER currentDamagedScript;
    private SpawnUncompletedCharacter currentSpawnScript;
    [Header("CAMERAS")]
    public GameObject VRCamera;
    public GameObject[] otherCameras;
    [Header("TUTORIAL")]
    public int tutorialStep;
    public AudioSource tutorialAudioSource;
    public CanvasGroup tutorialCanvasGroup;
    public CanvasGroup arrowCanvasGroup;
    
    [TextArea] public string[] tutorialTexts;
    public Transform[] tutorialTargets;
    public Transform[] arrowTargets;
    public AudioClip[] voiceClips;
    public TextMeshProUGUI textElement;
    private bool blackText;


    [Header("SETTINGS")]
    [SerializeField] int baseCrowdSize = 10;
    [SerializeField] float safeZoneRadius = 2f;
    [SerializeField] float maxRadius = 10f;
    public Vector3 repairPosition;
    public bool isTuto = false;
    public bool isVR = false;
    public int currentCameraIndex = 0;
    public float drawTimer;
    public float drawTimerCheck = 5f;

    private SYSTEM_CHARACTER tempScript;

    void Start()
    {
        CreateCrowd();
        CreateDamaged();

        //FONCTION POUR AGIR SI LE CASQUE EST MIS OU RETIRE
        //OVRManager.HMDUnmounted -= OnHMDMounted;
        //OVRManager.HMDUnmounted += OnHMDUnmounted;

        VRCamera.SetActive(false);
        for (int i = 0; i < otherCameras.Length; i++)
        {
        otherCameras[i].SetActive(i == currentCameraIndex);
        }

        //UI INVISIBLE AU DEBUT
        tutorialCanvasGroup.alpha = 0f;
        arrowCanvasGroup.alpha = 0f;
    }

    private void OnHMDMounted()
    {
        isVR = true;
    }
    private void OnHMDUnmounted()
    {
        isVR = false;
    }

    void Update()
    {
        if (currentDamaged != null)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.B) || Input.GetKeyDown(KeyCode.F))
            {
                currentDamagedScript.FinishDraw();
                currentDamaged = null;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                currentSpawnScript.isStuck = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            isVR = !isVR;
        }

        if (isVR)
        {
            VRCamera.SetActive(true);
             foreach (GameObject cam in otherCameras)
            {
                cam.SetActive(false);
            }
        }

        if (!isVR)
        {
            VRCamera.SetActive(false);
            
            for (int i = 0; i < otherCameras.Length; i++)
            {
                otherCameras[i].SetActive(i == currentCameraIndex);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwitchCamera(1); 
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwitchCamera(-1); 
            }
        }


        if (Input.GetKeyDown(KeyCode.N))
        {
            arrowSpriteScript.invertColors = !arrowSpriteScript.invertColors;
            arrowSpriteScript.ApplyInversion();

            boxSpriteScript.invertColors = !boxSpriteScript.invertColors;
            boxSpriteScript.ApplyInversion();
        }


        if (Input.GetKeyDown(KeyCode.T)){tutorialStep = 0; Tutorial();}
        if (Input.GetKeyDown(KeyCode.Y)){tutorialStep++; Tutorial();}


        if (drawZoneScript.canDraw)
        {
            drawTimer += Time.deltaTime;
        }

        //CONDITIONS DU TUTORIEL
        if (isTuto && tutorialStep == 0 && OVRInput.GetDown(OVRInput.RawButton.A)){tutorialStep++; Tutorial();}
        if (isTuto && tutorialStep == 0 && OVRInput.GetDown(OVRInput.RawButton.X)){tutorialStep++; Tutorial();}
        if (isTuto && tutorialStep == 1 && OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)){tutorialStep++; Tutorial();}
        if (isTuto && tutorialStep == 1 && OVRInput.GetDown(OVRInput.RawButton.LHandTrigger)){tutorialStep++; Tutorial();}
        if (isTuto && tutorialStep == 2 && drawTimer >= drawTimerCheck){tutorialStep++; Tutorial();}
        if (isTuto && tutorialStep == 2 && drawTimer >= drawTimerCheck){tutorialStep++; Tutorial();}
        if (isTuto && tutorialStep == 3 && OVRInput.GetDown(OVRInput.RawButton.B)){tutorialStep++; Tutorial();}
        if (isTuto && tutorialStep == 3 && OVRInput.GetDown(OVRInput.RawButton.Y)){tutorialStep++; Tutorial();}


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            passthrough.SetActive(!passthrough.activeSelf);
            arrowSpriteScript.invertColors = !arrowSpriteScript.invertColors;
            arrowSpriteScript.ApplyInversion();

            boxSpriteScript.invertColors = !boxSpriteScript.invertColors;
            boxSpriteScript.ApplyInversion();

            if (blackText) { 
                textElement.color = Color.black;
                blackText = false;
            }
            else
            {
                textElement.color = Color.white;
                blackText = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerObject.transform.position = center.position;
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            musicAudioSource.mute = !musicAudioSource.mute;
        }

    }

    void SwitchCamera(int direction)
    {
        otherCameras[currentCameraIndex].SetActive(false);

        currentCameraIndex += direction;
        if (currentCameraIndex < 0)
            currentCameraIndex = otherCameras.Length - 1;
        else if (currentCameraIndex >= otherCameras.Length)
            currentCameraIndex = 0;

        otherCameras[currentCameraIndex].SetActive(true);
    }

    public void CreateCrowd()
    {
        for (int i = 0; i < baseCrowdSize; i++)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(safeZoneRadius, maxRadius);
            Vector3 spawnPosition = center.position + new Vector3(randomPoint.x, 0, randomPoint.y);
            GameObject crowd = Instantiate(crowdPrefab, repairPosition, Quaternion.identity);
            GameObject character = Instantiate(characterPrefab, repairPosition, Quaternion.identity);
            character.GetComponent<SYSTEM_CHARACTER>().paintMaterial = Pencil.GetComponent<Pencil02>().coloredPaint;
            character.transform.SetParent(crowd.transform);

            tempScript = character.GetComponent<SYSTEM_CHARACTER>();
            tempScript.CreateFull();

            crowd.transform.position = spawnPosition;
        }
    }

    public void CreateDamaged()
    {
        pencilScript.CreateBaseMesh(); // Initie le dessin de la nouvelle partie
        currentDamaged = Instantiate(characterPrefab, repairPosition, Quaternion.identity);
        currentDamaged.GetComponent<SYSTEM_CHARACTER>().paintMaterial = Pencil.GetComponent<Pencil02>().coloredPaint;
        currentDamagedScript = currentDamaged.GetComponent<SYSTEM_CHARACTER>();
        currentDamagedScript.CreateDamaged();
        currentSpawnScript = currentDamaged.GetComponent<SpawnUncompletedCharacter>();

        currentDamaged.transform.position = repairSpawn.position;

        if (!isTuto)
        {
            currentSpawnScript.isStuck = false;
        }
    }

    public void Tutorial()
    {
        switch (tutorialStep)
        {
            case 0:
                drawTimer = 0f;
                isTuto = true;
                tutorialCanvasGroup.alpha = 1f;
                arrowCanvasGroup.alpha = 1f;
                if (currentDamaged != null)
                {
                    Destroy(currentDamaged);
                    CreateDamaged();
                }
                Pencil.transform.position = penBase.transform.position;
                //StartCoroutine(FadeCanvasGroup(tutorialCanvasGroup, 1f, 0.5f));
                //StartCoroutine(FadeCanvasGroup(arrowCanvasGroup, 1f, 0.5f));
                textTyperScript.fullText = tutorialTexts[0];
                textFollowCamScript.targetObject = tutorialTargets[0] != null ? tutorialTargets[0] : center;
                arrowFollowCamScript.targetObject = arrowTargets[0] != null ? arrowTargets[0] : center;
                tutorialAudioSource.PlayOneShot(voiceClips[0]);
                break;

            case 1:
                textTyperScript.fullText = tutorialTexts[1];
                textFollowCamScript.targetObject = tutorialTargets[1] != null ? tutorialTargets[1] : center;
                arrowFollowCamScript.targetObject = arrowTargets[1] != null ? arrowTargets[1] : center;
                tutorialAudioSource.PlayOneShot(voiceClips[1]);
                break;

            case 2:
                currentSpawnScript.isStuck = false;
                textTyperScript.fullText = tutorialTexts[2];
                textFollowCamScript.targetObject = tutorialTargets[2] != null ? tutorialTargets[2] : center;
                arrowFollowCamScript.targetObject = arrowTargets[2] != null ? arrowTargets[2] : center;
                tutorialAudioSource.PlayOneShot(voiceClips[2]);
                break;

            case 3:
                textTyperScript.fullText = tutorialTexts[3];
                textFollowCamScript.targetObject = tutorialTargets[3] != null ? tutorialTargets[3] : center;
                arrowFollowCamScript.targetObject = arrowTargets[3] != null ? arrowTargets[3] : center;
                tutorialAudioSource.PlayOneShot(voiceClips[3]);
                break;

            case 4:
                isTuto = false;
                tutorialStep = 0;
                tutorialCanvasGroup.alpha = 0f;
                arrowCanvasGroup.alpha = 0f;
                //StartCoroutine(FadeCanvasGroup(tutorialCanvasGroup, 0f, 0.5f));
                //StartCoroutine(FadeCanvasGroup(arrowCanvasGroup, 0f, 0.5f));
                break;

            default:
                break;
        }
    }



    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        cg.alpha = targetAlpha;

        if (targetAlpha == 0f)
        {
            Tutorial();  
        }
}
        
}
