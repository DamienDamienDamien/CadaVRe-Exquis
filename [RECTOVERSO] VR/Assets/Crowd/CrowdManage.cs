using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public class CrowdManage : MonoBehaviour
{
    public Vector2 SizeNavX = new Vector2(-20f, 20f);
    public Vector2 SizeNavY = new Vector2(-20f, 20f);
    public float radius = 10f;

    public float speedMin = 2f;
    public float speedMax = 7f;

    public float animTimeMin = 2f;
    public float animTimeMax = 10f;

    private int stepsBeforeActionMin = 0;
    private int stepsBeforeActionMax = 3;

    public UnityEngine.AI.NavMeshAgent navMesh;
    private Vector3 Target;
    public Animator animatorChild;
    private float StartTimerForTarget;
    private State currentState = State.Idle;
    private Dictionary<State, System.Action> stateActions;
    private int stepsBeforeAction;
    private int stepsTaken = 0;
    private bool canTalk = false;
    private float actionTime = 2f;
    private Transform lookTarget = null;

    void Awake()
    {
        stepsBeforeAction = Random.Range(stepsBeforeActionMin, stepsBeforeActionMax);
        stateActions = new Dictionary<State, System.Action>
    {
        { State.Waving, Waving },
        { State.Talking, Talking },
        { State.Other, Other },
        { State.Dancing, Dancing }
    };
        animatorChild = GetComponentInChildren<Animator>(true);
    }

    void Start()
    {
        RandomTarget();
        FindTarget();
        navMesh.speed = Random.Range(speedMin, speedMax);
    }

    void Update()
    {
        if (currentState != State.Idle) return;

        float distTarget = Vector3.Distance(navMesh.transform.position, Target);
        float elapsedTime = Time.time - StartTimerForTarget;

        if (distTarget < 1f)
        {
            animatorChild.SetBool("isWalking", false);
            stepsTaken++;

            if (stepsTaken == stepsBeforeActionMax) {
                stepsTaken = 0;
                stepsBeforeAction = Random.Range(stepsBeforeActionMin, stepsBeforeActionMax);
                //print("nb de pas avant action : " + stepsBeforeAction);
                ChooseRandomState();
            }
            else
            {
                FindTarget();
            }
        }
        else if (elapsedTime > 20f)
        {
            //Debug.Log("Trop long changement de cible.");
            FindTarget();
        }
    }
    private IEnumerator PerformState(State newState, float duration)
    {
        currentState = newState;
        navMesh.isStopped = true;

        if (stateActions.TryGetValue(newState, out var action))
        {
            action?.Invoke();
        }

        float timer = 0f;
        while (timer < duration)
        {
            if (newState == State.Talking && lookTarget != null)
            {
                Vector3 direction = (lookTarget.position - transform.position).normalized;
                direction.y = 0; // pour ne pas regarder vers le haut/bas
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
        navMesh.isStopped = false;
        currentState = State.Idle;
        lookTarget = null;
        FindTarget();
    }


    public void FindTarget()
    {
        ResetAllAnimatorBools();
        animatorChild.SetBool("isWalking", true);
        Target = RandomTarget();
        navMesh.destination = Target;
        StartTimerForTarget = Time.time;
    }

    public Vector3 RandomTarget()
    {
        Vector3 randomPoint = new Vector3 (Random.Range(SizeNavX.x, SizeNavX.y), 0, Random.Range(SizeNavY.x, SizeNavY.y));
        return randomPoint;
    }

    public void Waving()
    {
        animatorChild.SetBool("isWaving", true);
        //print("Salut");
    }
    public void Dancing()
    {
        //print("je Dance");
        animatorChild.SetBool("isDancing", true);
    }

    public void Talking()
    {
        animatorChild.SetBool("isTalking", true);
        //print("je parle");
    }

    public void Other()
    {
        //print("Autre");
    }

    public enum State
    {
        Idle,
        Waving,
        Talking,
        Other,
        Dancing
    }

    void ChooseRandomState()
    {
        List<State> possibleStates = new List<State>();

        if (canTalk)
        {
            possibleStates.Add(State.Talking);
        }
        possibleStates.Add(State.Waving);
        possibleStates.Add(State.Other);
        possibleStates.Add(State.Dancing);

        int rand = Random.Range(0, possibleStates.Count);
        State chosen = possibleStates[rand];

        float randomDuration = Random.Range(animTimeMin, animTimeMax);
        StartCoroutine(PerformState(chosen, randomDuration));

        if (chosen == State.Talking)
        {
            canTalk = false;
        }

    }

    void ResetAllAnimatorBools()
    {
        if (animatorChild == null)
        {
            animatorChild = GetComponentInChildren<Animator>(true);
        }
        foreach (AnimatorControllerParameter param in animatorChild.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                animatorChild.SetBool(param.name, false);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        CrowdManage otherCrowd = other.GetComponent<CrowdManage>();

        if (otherCrowd != null && otherCrowd != this)
        {
            ResetAllAnimatorBools();
            otherCrowd.ResetAllAnimatorBools();
            //print("je rentre en collision avec " + other.name);

            float chance = Random.value;
            if (chance < 0.70f)
            {
                if (otherCrowd.currentState == State.Idle)
                {
                    lookTarget = other.transform;
                    otherCrowd.lookTarget = this.transform;

                    float duration = Random.Range(animTimeMin, animTimeMax);
                    StartCoroutine(PerformState(State.Talking, duration));

                    float delay = Random.Range(0.1f, 0.4f);
                    otherCrowd.StartCoroutine(otherCrowd.PerformState(State.Talking, actionTime));
                }
            }
        }
    }
}


