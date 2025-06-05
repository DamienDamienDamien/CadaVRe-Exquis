using UnityEngine;
using System.Collections;
public class SpawnUncompletedCharacter : MonoBehaviour
{
    [Header("REFERENCES")]
    public GameObject prefabParent;
    public AnimatorOverrideController overrideController;
    public Camera mainCamera;

    public Transform cible;
    public Transform cible2;
    public SYSTEM_GAME gameScript;
    public SYSTEM_CHARACTER characterScript;

    [Header("SETTINGS")]
    public float vitesse = 5f;

    public float distanceArret = 0f;
    private int behaviourStep = 0; // 0: d�part, 1: arriv� � la cible 1, 2 : repartir; 3 : arriv� � la cible2, Rejoindre la foule
    private Animator animator;
    public bool isStuck = true;
    private float distance;

    public bool isCompleted = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>(true);
        mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
        cible = GameObject.FindGameObjectWithTag("Cible").transform;
        cible2 = GameObject.FindGameObjectWithTag("Cible2").transform;
        gameScript = GameObject.FindWithTag("gameManager").GetComponent<SYSTEM_GAME>();
        characterScript = GetComponent<SYSTEM_CHARACTER>();

    }
    void Update()
    {
        if (!isStuck)
        {
            switch (behaviourStep)
            {
                case 0: // d�part
                    gototarget(cible);
                    break;
                case 1: // arriv� � la cible 1
                    lookatTarget(mainCamera.transform);
                    break;
                case 2: // repartir
                    gototarget(cible2);
                    break;
                case 3: // arriv� � la cible 2
                    SpawnAndSetAsParent();
                    gameScript.CreateDamaged();
                    behaviourStep++;
                    // faire des trucs
                    break;
            }
        }
    }

    public void gototarget(Transform target)
    {
        if (cible == null) return;

        distance = Vector3.Distance(transform.position, target.position);

            animator.SetBool("isWalking", true);
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * vitesse * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                Quaternion rotationtarget = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationtarget, Time.deltaTime *vitesse);
            }
        if (distance <= distanceArret){
            gameObject.transform.position = target.position;
            gameObject.transform.rotation = target.rotation;
            behaviourStep++;
            animator.SetBool("isWalking", false);
            animator.SetBool("isTpose", true);
            characterScript.drawZone.SetActive(true);
            characterScript.MoveDrawBox();
        }
    }

    public void lookatTarget (Transform target)
    {
        //Vector3 directionCam = target.transform.position - transform.position;
            //directionCam.y = 0f;
            //if (directionCam != Vector3.zero)
            //{
                //Quaternion rotationVersCam = Quaternion.LookRotation(directionCam);
                //transform.rotation = Quaternion.Slerp(transform.rotation, rotationVersCam, Time.deltaTime * 5f);
                //animator.SetBool("isWalking", false);
               // animator.SetBool("isTpose", true);

        //}

        if (isCompleted)
            {
            animator.SetBool("isTpose", false);
            behaviourStep++;
            }
    }

    public void SpawnAndSetAsParent()
    {
        if (prefabParent == null)
        {
            Debug.LogWarning("Prefab parent non assign� !");
            return;
        }

        GameObject instance = Instantiate(prefabParent, transform.position, Quaternion.identity);
        transform.SetParent(instance.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        CrowdAnimation crowdAnim = instance.GetComponent<CrowdAnimation>();
        if (crowdAnim != null)
        {
            crowdAnim.overrideControllerTemplate = overrideController;
            crowdAnim.Initialize();
        }

        this.enabled = false;
    }





}
